## Experiment data -------------------------

initialBacklogSize = 15
initialCodedSize = 0
maxTime = 30.0 # days
meanDevTime = 10.5 # mean, days
varDevTime = 0.1 # variation (the less the better process)
quality = 1 # probability of reopen
meanTestTime = 6.0 # mean, days
meanUsArrival = 2.0 # mean, days
developerCount = 5
qaCount = 2

from simulator import run

data = run({"theseed": 99999, "initialBacklogSize": initialBacklogSize, "maxTime": maxTime, "meanDevTime": meanDevTime,
          "varDevTime": varDevTime, "meanTestTime": meanTestTime, "meanUsArrival": meanUsArrival,
          "developerCount": developerCount, "qaCount": qaCount, "initialCodedSize": initialCodedSize,
          "quality": quality})
print data
input('press a key')