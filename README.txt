Chandler Butler
Project 2 CSCI 3500
************************Running the program*****************
1) Run the exe located in Project2\bin\debug\Project2.exe

2) The program will prompt you for a filepath to a .TM file
   and will let you know if your filepath was bad or your
   permision level was bad. File path should look something like
   this with extra attention on .TM.txt being at the end of your filepath
   (IE. C:\Users\Chand\Desktop\Master Folder\getrepository\Project2\Testmachine.TM.txt)

3) Insert a word and the console will output the steps the
   turingmachine is taking in real time, and will stop at
   a reject or accept state and tell you if your word was
   accepted.
********************.TM file semantics******************
 * In the .TM file states are case insensitive
   (IE. {states: q0,Q1} == {states : Q0,Q1})
   this property follows through to the start, reject, and 
   accept states. Also in states no duplicate states can be
   inserted(IE. {states: Q0,Q0} will prompt you to try again)

 * Alpha must be a subset of the tape-alpha line and these lines
   are case sensitive.

 * Tape of the machine is only infinite to the right. If you
   push the header off of the left side the program will let you
   know and then ask you to try again.

 * -- are comments, so everything to the right of a -- is 
      completly ignored when parsed.

 * An example .TM file is located in the repository in the same directory this
   readme is located.