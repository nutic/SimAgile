import sys
sys.path.append('c:/Users/shnyukova/Documents/Visual Studio 2012/Projects/SimAgile/packages/IronPython.2.7.3/lib/Net40/')
# from SimPy.SimulationTrace import *
from random import expovariate, seed, normalvariate, uniform
from SimPy.Simulation import *

def random_index(probabilities):
    x = uniform(0, 1)
    cumulative_probability = 0.0
    for i, item_probability in enumerate(probabilities):
        cumulative_probability += item_probability
        if x < cumulative_probability: break
    return i

def const(val):
    return lambda: val

def run(parameters):
    class Source(Process):
        def __init__(self, name):
            self.i = 0
            return super(Source, self).__init__(name=name)

        def generateBacklog(self, count):
           limit = self.i + count
           while self.i < limit:
                us = UserStory(name="UserStory%02d" % (self.i,))
                activate(us, us.plan())
                self.i += 1

        def generateCoded(self, count):
           limit = self.i + count
           while self.i < limit:
                us = UserStory(name="UserStory%02d" % (self.i,))
                us.changeState("Planned")
                us.changeState("In Dev")
                activate(us, us.coded())
                self.i += 1

        def start(self, meanUsArrival):
            while (True):
                us = UserStory(name="UserStory%02d" % (self.i,))
                activate(us, us.plan())
                yield hold, self, (expovariate(1.0 / meanUsArrival))
                self.i += 1

    class DayMonitor(Process):
        def __init__(self):
            self.log = dict(zip(states, map(lambda x: [], states)));
            self.log["WIP"] = []
            return super(DayMonitor, self).__init__()

        def _state(self, state):
            return stores[state].nrBuffered

        def _wip(self):
            return len(filter(lambda us: us.startDate is not None and us.endDate is None, work))
        
        def start(self):
            while (True):
                for state in states:
                    self.log[state].append(self._state(state))
                self.log["WIP"].append(self._wip())
                yield hold, self, 1;

    class Person(Process):
        def __init__(self, name, role, time):
            self.role = role;
            self.time = time;
            return super(Person, self).__init__(name=name)
        
        def start(self):
            while True:
                for income in process[self.role].income:
                    yield (get, self, stores[income], 1), (hold, self, 0)
                    if self.acquired(stores[income]):
                        break;
                
                if len(self.got) == 0:
                    yield get, self, stores[income], 1
                
                unit = self.got[0]
                
                wip = process[self.role].wip
                yield unit.changeState(wip)
                yield hold, self, self.time()
                
                yield get, self, stores[wip], lambda x: [x[x.index(unit)]]
                
                outcome = process[self.role].outcome
                index = random_index(map(lambda x: x["p"](), outcome))
                yield unit.changeState(outcome[index]["state"])

    class UserStory(Process):
        def __init__(self, name):
            self.state = 'None'
            self.changeStateDate = 0
            self.timeInState = {}
            self.leadTime = None
            self.cycleTime = None
            self.createDate = None
            self.startDate = None
            self.endDate = None
            
            work.append(self)
            
            return super(UserStory, self).__init__(name=name)
        
        def plan(self):
            yield self.changeState("Planned")

        def coded(self):
            yield self.changeState("Coded")

        def create(self):
            self.createDate = now()

        def start(self):
            self.startDate = now()

        def end(self):
            self.endDate = now()
            self.leadTime = self.endDate - self.createDate
            self.cycleTime = self.endDate - self.startDate

        def _registerStateChange(self, state):
            if self.state not in self.timeInState:
                self.timeInState[self.state] = 0
            
            self.timeInState[self.state] += now() - self.changeStateDate
            
            self.state = state
            self.changeStateDate = now()

        def changeState(self, state):
            self._registerStateChange(state)
            
            def skip():
                return            
            {
                "Planned": self.create,
                "In Dev": self.start,
                "Done": self.end,
            }.get(state, lambda: skip)()

            return put, self, stores[state], [self]

        def short(self):
            return {"name": self.name,
                    "leadTime": self.leadTime,
                    "cycleTime": self.cycleTime,
                    "timeInState": self.timeInState,
                    "createDate": self.createDate,
                    "startDate": self.startDate,
                    "endDate": self.endDate }

    class ProcessDescription():
        def __init__(self, income, wip, outcome):
            self.income = income
            self.wip = wip
            self.outcome = outcome

    class QualityDropEvent(Process):
        def raiseIt(self):
            parameters["quality"] = parameters["quality"] / 3.0

    class DeveloperLeftEvent(Process):
        def raiseIt(self):
            for i in range(0, parameters["developerCount"]/2):
                Process().cancel(team[i])

    class EventSource(Process):
        def __init__(self, evt, at):
            self.evt = evt
            self.at = at
            return super(EventSource, self).__init__()

        def activate(self):
            yield hold, self, self.at
            self.evt.raiseIt()

    # configuration of process
    states = [
              "Planned",
              "In Dev",
              #"Reopen",
              "Coded",
              "Testing",
              "Done"]
    stores = dict(zip(states, map(lambda x: Store(name=x), states)))
    work = []
    process = {"Dev": ProcessDescription(
                                         #income=[stores["Reopen"], stores["Planned"]],
                                         income=["Planned"],
                                         wip="In Dev",
                                         outcome=[{"state":"Coded", "p": const(1)}]),
               "QA": ProcessDescription(
                                        income=["Coded"],
                                        wip="Testing",
                                        outcome=[{"state": "Done", "p": lambda: parameters["quality"]}, {"state": "In Dev", "p": lambda: 1 - parameters["quality"]}])}

    seed(parameters["theseed"])
    initialize()

    # simulation activation
    source = Source('Source')
    source.generateBacklog(parameters["initialBacklogSize"])
    source.generateCoded(parameters["initialCodedSize"])
    activate(source, source.start(meanUsArrival=parameters["meanUsArrival"]), at=0.0)

    # evtSources = [EventSource(evt=QualityDropEvent(), at=parameters["maxTime"]/2)]
    # evtSources = [EventSource(evt=DeveloperLeftEvent(), at=parameters["maxTime"]/2)]
    evtSources = []
    for evtSource in evtSources:
        activate(evtSource, evtSource.activate(), at=0.0)

    monitor = DayMonitor()
    activate(monitor, monitor.start(), at=0.0)
    team = map(lambda i: Person(name="Dev%02d" % i, role="Dev", time = lambda: normalvariate(parameters["meanDevTime"], parameters["varDevTime"])), range(0, parameters["developerCount"])) + \
        map(lambda i: Person(name="QA%02d" % i, role="QA", time = lambda: expovariate(1.0/parameters["meanTestTime"])), range(0, parameters["qaCount"]))
    for person in team:
        activate(person, person.start(), at=0.0)
    
    # simulation start
    simulate(until=parameters["maxTime"])
    
    # remove 'Done' state
    # del monitor.log[states[len(states)-1]]
    # states.remove(states[len(states)-1])
    return {"states": states, "history": monitor.log, "work": map(lambda unit: unit.short(), work), "done": map(lambda unit: unit.short(), stores["Done"].theBuffer)};