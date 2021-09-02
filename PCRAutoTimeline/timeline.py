from autotimeline import *

count = 0;
def asynctest1():
    global count
    print("async1 starting")
    while True:
        Async.Await()
        count=count+1
        if count % 1000 == 0:
            print(count)
            return
        
def asynctest2():
    global count
    print("async2 starting")
    while True:
        Async.Await()
        count=count+1
        if count % 1000 == 0:
            print(count)
            return

def asynctest3():
    global count
    print("async3 starting")
    while True:
        Async.Await()
        count=count+1
        if count % 1000 == 0:
            print(count)
            return

Async.Start(asynctest1)
Async.Start(asynctest2)
Async.Start(asynctest3)

Async.Exit()
'''
print("asynctest stated");
while True:
    Async.Await()
    count=count+1
    if count % 300 == 1:
        print(count)
'''