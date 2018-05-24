using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    class TuringMachine
    {
        public HashSet<State> states { get; set; }
        public string inputAlphabet { get; set; }
        public string tapeAlphabet { get; set; }
        public State acceptState { get; set; }
        public State rejectState { get; set; }
        public State startState { get; set; }
        public Dictionary<State, List<Edge>> transitionFunction { get; set; }
        public State currentState { get; set; }

        public string[] tape { get; set; }
        public int header { get; set; }
        //CONSTRUCTOR
        public TuringMachine(string filePath)
        {
            createMachine(filePath);
        }

        public TuringMachine()
        {
            throw new FormatException("No file path to .TM file found");
        }

        public void run(TuringMachine machine)
        {
            currentState = startState;
            header = 1;

            tape = setTape();
            showTape();

            bool foundEdge;
            while (currentState != acceptState || currentState != rejectState)
            {
                foundEdge = false;
                List<Edge> potentialEdges = new List<Edge>();
                try { potentialEdges = transitionFunction[currentState]; }
                catch
                {
                    currentState = rejectState;
                    continue;
                }
                
                foreach(Edge x in potentialEdges)
                {
                    if (potentialEdges.Count == 0)
                        currentState = rejectState;

                    if(x.read.ToString().Equals(tape[header]))
                    {
                        tape[header] = x.write.ToString();
                        if (x.moveHeader == 'R')
                        {
                            if (header < tape.Count() - 2)
                            {
                                header++;
                                tape[header - 2] = tape[header - 1];
                                tape[header - 1] = "[" + x.nextState.name + "]";
                            }
                            else
                            {
                                resizeArray(tape);
                                header++;
                                tape[header - 2] = tape[header - 1];
                                tape[header - 1] = "[" + x.nextState.name + "]";
                            }
                            
                        }
                        else
                        {
                            if (header > 1)
                            {
                                header--;
                                tape[header] = tape[header - 1];
                                tape[header - 1] = "[" + x.nextState.name + "]";
                                //header--;
                            }
                            else
                            {
                                throw new FormatException("Header fell off the front of the tape. Only infinite in the other direction.");
                            }
                        }
                        currentState = x.nextState;
                        foundEdge = true;
                        break;
                    }
                    
                }
                if (!foundEdge)
                    currentState = rejectState;
                Console.Write("\n");
                if (currentState == acceptState)
                    Console.Write("Accept: ");
                    
                else if (currentState == rejectState)
                    Console.Write("Reject: " );

                showTape();
            }
        }

        public void resizeArray(string[] input)
        {
            string[] temp = new string[tape.Count() + 1];
            temp[temp.Count() - 1] = "_";
            int index = 0;

            foreach(string x in tape)
            {
                temp[index] = tape[index];
                index++;
            }

            tape = temp;
        }

        public string[] setTape()
        {
            Console.Write("Enter a word: ");
            string inputWord = Console.ReadLine();

            bool verified = wordVerifier(inputWord);

            while (!verified)
            {
                Console.WriteLine("Invalid word");
                Console.WriteLine("Enter a word: ");
                inputWord = Console.ReadLine();
                verified = wordVerifier(inputWord);
            }
            string[] output = new string[inputWord.Length + 1];
            output[0] = "[" + startState.name + "]";

            for(int i = 1; i <= output.Length - 1; i++)
            {
                
                    output[i] = inputWord[i - 1].ToString();
                
            }

            return output;
        }
        
        public bool wordVerifier(string inputWord)//TO BE IMPLEMENTED
        {
            // Must contain only letter of the input alphabet
            foreach(char x in inputWord)
            {
                if (!(inputAlphabet.Contains(x)))
                    return false;
            }

            return true;
        }

        public void showTape()//TO BE IMPLEMENTED
        {
            
            foreach (string x in tape)
            {
                
                Console.Write(x);
            }

        }

        //Creates the Turing Machine and sets all of its transitions
        public void createMachine(string filepath)
        {
            states = new HashSet<State>();
            inputAlphabet = "";
            tapeAlphabet = "";
            transitionFunction = new Dictionary<State, List<Edge>>(); 
            
        string[] inputFile = endTrimmer(blankLineCleaner(commentCleaner(System.IO.File.ReadAllLines(@filepath))));
            //Initializes and verifies that a proper turing was created
            for(int i = 0; i < 6; i++)
            {
                switch(i)
                {
                    case 0:
                        verifyStates(inputFile[i]);
                        insertStates(inputFile[i]);
                        break;
                    case 1:
                        verifyStart(inputFile[i]);
                        insertStart(inputFile[i]);
                        break;
                    case 2:
                        verifyAccept(inputFile[i]);
                        insertAccept(inputFile[i]);
                        break;
                    case 3:
                        verifyReject(inputFile[i]);
                        insertReject(inputFile[i]);
                        break;
                    case 4:
                        verifyTapealpha(inputFile[i + 1]);
                        insertTapealpha(inputFile[i + 1]);
                        break;
                    case 5:
                        verifyAlpha(inputFile[i - 1]);
                        insertAlpha(inputFile[i - 1]);
                        break;
                }
            }

            for(int i = 6; i < inputFile.Length; i++)
            {
                switch (inputFile[i].Substring(0, 4).ToLower())
                {
                    case "rwrt":
                        verifyRWRT(inputFile[i]);
                        addRWRT(inputFile[i]);
                        break;
                    case "rwlt":
                        verifyRWLT(inputFile[i]);
                        addRWLT(inputFile[i]);
                        break;
                    case "rrl ":
                        verifyRRL(inputFile[i]);
                        addRRL(inputFile[i]);
                        break;
                    case "rll ":
                        verifyRLL(inputFile[i]);
                        addRLL(inputFile[i]);
                        break;
                    case "rrt ":
                        verifyRRT(inputFile[i]);
                        addRRT(inputFile[i]);
                        break;
                    case "rlt ":
                        verifyRLT(inputFile[i]);
                        addRLT(inputFile[i]);
                        break;
                    default:
                        throw new FormatException("Invalid command encountered");
                }
            }

            for(int i = 0; i < states.Count() - 1; i++)
            {
                try { states.ElementAt(i).edges = transitionFunction[states.ElementAt(i)]; }
                catch (KeyNotFoundException) { }
            }

            
        }
        //Strips out all comments
        public string[] commentCleaner(string[] input)
        {
            string[] output = input;
            int i = 0;
            foreach(string x in input)
            {
                int y = 0;
                while(y < x.Length)
                {
                    if(x[y] == '-' && x[y+1] == '-')
                    {
                        output[i] = x.Substring(0, y);
                        break;
                    }
                    y++;
                }
                i++;
            }

            return output;
        }

        //Cleans up the blank lines from a file
        public string[] blankLineCleaner(string[] input)
        {
            string[] output = new string[input.Length];
            int i = 0;
            int y = 0;
            foreach (string x in input)
            {
                if(!(x.Length == 0))
                {
                    output[i] = input[y];
                    i++;
                }
                y++;
                
            }

            return output;
        }

        //Trims the whitespace of strings in the array and removes null parts of the array
        public string[] endTrimmer(string[] input)
        {
            string[] output = new string[input.Length];
            int i = 0;

            foreach(string x in input)
            {
                if (x != null)
                {
                    output[i] = x.TrimEnd();
                    i++;
                }
            }

            output = output.ToList().Where(z => z != null).ToArray();

            return output;
        }

        //Verifying and adding the commands
        public void verifyRWRT(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[4] = rawData[4].Substring(0, rawData[4].Length - 1);

            if (rawData.Length != 5)
                throw new FormatException("Missing or too many arguments");

            if (input[input.Length -1] != ';')
                throw new FormatException("Missing ;");

            bool qSeen = false;
            bool qprimeSeen = false;

            foreach (State x in states)
            {
                if (x.name == rawData[1])
                    qSeen = true;

                if (x.name == rawData[4])
                    qprimeSeen = true;
            }

            if (!qSeen)
                throw new FormatException("q is not in the set of states.");
            if (!qprimeSeen)
                throw new FormatException("q' is not in the set of states.");

            if ((!tapeAlphabet.Contains(rawData[2])) || (!tapeAlphabet.Contains(rawData[3])))
                throw new FormatException("Read or Write character is not in the tape alphabet.");

        }

        public void verifyRWLT(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[4] = rawData[4].Substring(0, rawData[4].Length - 1);

            if (rawData.Length != 5)
                throw new FormatException("Missing or too many arguments");

            if (input[input.Length - 1] != ';')
                throw new FormatException("Missing ;");

            bool qSeen = false;
            bool qprimeSeen = false;

            foreach (State x in states)
            {
                if (x.name == rawData[1])
                    qSeen = true;

                if (x.name == rawData[4])
                    qprimeSeen = true;
            }

            if (!qSeen)
                throw new FormatException("q is not in the set of states.");
            if (!qprimeSeen)
                throw new FormatException("q' is not in the set of states.");

            if ((!tapeAlphabet.Contains(rawData[2])) || (!tapeAlphabet.Contains(rawData[3])))
                throw new FormatException("Read or Write character is not in the tape alphabet.");
        }

        public void verifyRRL(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[2] = rawData[2].Substring(0, rawData[2].Length - 1);

            if (rawData.Length != 3)
                throw new FormatException("Missing or too many arguments");

            if (input[input.Length - 1] != ';')
                throw new FormatException("Missing ;");

            bool qSeen = false;
           
            foreach (State x in states)
            {
                if (x.name == rawData[1])
                    qSeen = true;
            }

            if (!qSeen)
                throw new FormatException("q is not in the set of states.");

            if (!tapeAlphabet.Contains(rawData[2]))
                throw new FormatException("Read character is not in the tape alphabet.");
        }

        public void verifyRLL(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[2] = rawData[2].Substring(0, rawData[2].Length - 1);

            if (rawData.Length != 3)
                throw new FormatException("Missing or too many arguments");

            if (input[input.Length - 1] != ';')
                throw new FormatException("Missing ;");

            bool qSeen = false;

            foreach (State x in states)
            {
                if (x.name == rawData[1])
                    qSeen = true;
            }

            if (!qSeen)
                throw new FormatException("q is not in the set of states.");

            if (!tapeAlphabet.Contains(rawData[2]))
                throw new FormatException("Read character is not in the tape alphabet.");
        }

        public void verifyRRT(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[3] = rawData[3].Substring(0, rawData[3].Length - 1);

            if (rawData.Length != 4)
                throw new FormatException("Missing or too many arguments");

            if (input[input.Length - 1] != ';')
                throw new FormatException("Missing ;");

            bool qSeen = false;
            bool qprimeSeen = false;

            foreach (State x in states)
            {
                if (x.name == rawData[1])
                    qSeen = true;

                if (x.name == rawData[3])
                    qprimeSeen = true;
            }

            if (!qSeen)
                throw new FormatException("q is not in the set of states.");
            if (!qprimeSeen)
                throw new FormatException("q' is not in the set of states.");

            if (!tapeAlphabet.Contains(rawData[2]))
                throw new FormatException("Read character is not in the tape alphabet.");
        }

        public void verifyRLT(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[3] = rawData[3].Substring(0, rawData[3].Length - 1);

            if (rawData.Length != 4)
                throw new FormatException("Missing or too many arguments");

            if (input[input.Length - 1] != ';')
                throw new FormatException("Missing ;");

            bool qSeen = false;
            bool qprimeSeen = false;

            foreach (State x in states)
            {
                if (x.name == rawData[1])
                    qSeen = true;

                if (x.name == rawData[3])
                    qprimeSeen = true;
            }

            if (!qSeen)
                throw new FormatException("q is not in the set of states.");
            if (!qprimeSeen)
                throw new FormatException("q' is not in the set of states.");

            if (!tapeAlphabet.Contains(rawData[2]))
                throw new FormatException("Read character is not in the tape alphabet.");
        }

        public void addRWRT(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[4] = rawData[4].Substring(0, rawData[4].Length - 1);

            foreach (State x in states)
            {
                foreach(State y in states)
                {
                    if (y.name.Equals(rawData[1]) && x.name.Equals(rawData[4]))
                        try { transitionFunction.Add(y, new List<Edge>(new Edge[] { new Edge(rawData[3][0], rawData[2][0], x, rawData[0][2]) })); }
                        catch {
                            List<Edge> sumTotal = new List<Edge>(new Edge[] { new Edge(rawData[3][0], rawData[2][0], x, rawData[0][2]) });
                            foreach(Edge z in transitionFunction[y])
                            {
                                sumTotal.Add(z);
                            }
                            
                            transitionFunction[y] = sumTotal; 
                        }
                }
            }
        }

        public void addRWLT(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[4] = rawData[4].Substring(0, rawData[4].Length - 1);

            foreach (State x in states)
            {
                foreach (State y in states)
                {
                    if (y.name.Equals(rawData[1]) && x.name.Equals(rawData[4]))
                        try { transitionFunction.Add(y, new List<Edge>(new Edge[] { new Edge(rawData[3][0], rawData[2][0], x, rawData[0][2]) })); }
                        catch
                        {
                            List<Edge> sumTotal = new List<Edge>(new Edge[] { new Edge(rawData[3][0], rawData[2][0], x, rawData[0][2]) });
                            foreach (Edge z in transitionFunction[y])
                            {
                                sumTotal.Add(z);
                            }

                            transitionFunction[y] = sumTotal;
                        }
                }
            }
        }

        public void addRRL(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[2] = rawData[2].Substring(0, rawData[2].Length - 1);

            foreach(State x in states)
            {
                if (x.name.Equals(rawData[1]))
                    try { transitionFunction.Add(x, new List<Edge>(new Edge[] { new Edge(rawData[2][0], rawData[2][0], x, rawData[0][1]) })); }
                    catch
                    {
                        List<Edge> sumTotal = new List<Edge>(new Edge[] { new Edge(rawData[2][0], rawData[2][0], x, rawData[0][1]) });
                        foreach (Edge z in transitionFunction[x])
                            sumTotal.Add(z);

                        transitionFunction[x] = sumTotal;
                    }
            }

        }

        public void addRLL(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[2] = rawData[2].Substring(0, rawData[2].Length - 1);

            foreach (State x in states)
            {
                if (x.name.Equals(rawData[1]))
                    try { transitionFunction.Add(x, new List<Edge>(new Edge[] { new Edge(rawData[2][0], rawData[2][0], x, rawData[0][1]) })); }
                    catch
                    {
                        List<Edge> sumTotal = new List<Edge>(new Edge[] { new Edge(rawData[2][0], rawData[2][0], x, rawData[0][1]) });
                        foreach (Edge z in transitionFunction[x])
                            sumTotal.Add(z);

                        transitionFunction[x] = sumTotal;
                    }
            }
        }

        public void addRRT(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[3] = rawData[3].Substring(0, rawData[3].Length - 1);

            foreach (State x in states)
            {
                foreach (State y in states)
                {
                    if (y.name.Equals(rawData[1]) && x.name.Equals(rawData[3]))
                        try { transitionFunction.Add(y, new List<Edge>(new Edge[] { new Edge(rawData[3][0], rawData[2][0], x, rawData[0][1]) })); }
                        catch
                        {
                            List<Edge> sumTotal = new List<Edge>(new Edge[] { new Edge(rawData[2][0], rawData[2][0], x, rawData[0][1]) });
                            foreach (Edge z in transitionFunction[y])
                            {
                                sumTotal.Add(z);
                            }

                            transitionFunction[y] = sumTotal;
                        }
                }
            }

        }

        public void addRLT(string input)
        {
            string[] rawData = input.Split(' ');
            rawData[3] = rawData[3].Substring(0, rawData[3].Length - 1);

            foreach (State x in states)
            {
                foreach (State y in states)
                {
                    if (y.name.Equals(rawData[1]) && x.name.Equals(rawData[3]))
                        try { transitionFunction.Add(y, new List<Edge>(new Edge[] { new Edge(rawData[3][0], rawData[2][0], x, rawData[0][1]) })); }
                        catch
                        {
                            List<Edge> sumTotal = new List<Edge>(new Edge[] { new Edge(rawData[2][0], rawData[2][0], x, rawData[0][1]) });
                            foreach (Edge z in transitionFunction[y])
                            {
                                sumTotal.Add(z);
                            }

                            transitionFunction[y] = sumTotal;
                        }
                }
            }
        }

        //Verifying and adding the configuration
        public void verifyStates(string input)
        {
            if (input[0] != '{' || input[input.Length - 1] != '}')
                throw new FormatException("Unbalanced Paranthesis on States");

            if (!(input.Substring(1, 7).ToLower().Equals("states:")))
                throw new FormatException("states line did not start with state:");

            string[] rawData = input.Substring(8,input.Length - 9).Split(',');
            rawData[0].TrimStart();

            foreach(string x in rawData)
            {
                int xEncountered = 0;
                foreach(string y in rawData)
                {
                    if (x.ToLower().Equals(y.ToLower()))
                        xEncountered++;

                }
                if (xEncountered > 1)
                    throw new FormatException("State is a set and can not contain duplicates.");
            }
        }

        public void verifyStart(string input)
        {
            if (input[0] != '{' || input[input.Length - 1] != '}')
                throw new FormatException("Unbalanced Paranthesis on Start");

            if (!(input.Substring(1, 6).ToLower().Equals("start:")))
                throw new FormatException("start line did not start with start:");

            string stateName = input.Substring(7,input.Length - 8).TrimStart();

            bool found = false;
            foreach (State x in states)
            {
                if (x.name.Equals(stateName))
                    found = true;
            }

            if (!found)
                throw new FormatException("Start state was not in the list of states");

        }

        public void verifyAccept(string input)
        {
            if (input[0] != '{' || input[input.Length - 1] != '}')
                throw new FormatException("Unbalanced Paranthesis on accept");

            if (!(input.Substring(1, 7).ToLower().Equals("accept:")))
                throw new FormatException("accept line did not start with accept:");

            string stateName = input.Substring(8, input.Length - 9).TrimStart();

            bool found = false;
            foreach (State x in states)
            {
                if (x.name.Equals(stateName))
                    found = true;
            }

            if (!found)
                throw new FormatException("accept state was not in the list of states");
        }

        public void verifyReject(string input)
        {
            if (input[0] != '{' || input[input.Length - 1] != '}')
                throw new FormatException("Unbalanced Paranthesis on reject");

            if (!(input.Substring(1, 7).ToLower().Equals("reject:")))
                throw new FormatException("reject line did not start with reject:");

            string stateName = input.Substring(8, input.Length - 9).TrimStart();

            bool found = false;
            foreach (State x in states)
            {
                if (x.name.Equals(stateName))
                    found = true;
            }

            if (!found)
                throw new FormatException("Reject state was not in the list of states");
        }

        public void verifyAlpha(string input)
        {
            if (input[0] != '{' || input[input.Length - 1] != '}')
                throw new FormatException("Unbalanced Paranthesis on alpha");

            if (!(input.Substring(1, 6).ToLower().Equals("alpha:")))
                throw new FormatException("alpha line did not start with alpha:");

            string alphabet = input.Substring(7, input.Length - 8).TrimStart();

            foreach(char x in alphabet)
            {
                if (x == '_')
                    throw new FormatException("Alphabet can not contain the '_' symbol");
            }

            if (alphabet.Length > 1 && alphabet[0] == ',' && alphabet[1] != ',')
                throw new FormatException("Alphas list must start with a proper character");

            for(int i = 1; i < alphabet.Length; i+=2)
            {
                if (alphabet[i] != ',')
                    throw new FormatException("Comma seperated list contains a string and not a char");
            }

            if (alphabet[alphabet.Length - 1] == ',')
                throw new FormatException("alpha's list does not end in a character");
        }

        public void verifyTapealpha(string input)
        {
            if (input[0] != '{' || input[input.Length - 1] != '}')
                throw new FormatException("Unbalanced Paranthesis on tapealpha");

            if (!(input.Substring(1, 11).ToLower().Equals("tape-alpha:")))
                throw new FormatException("alpha line did not start with tapealpha:");

            string alphabet = input.Substring(12, input.Length - 13).TrimStart();

            if (alphabet.Length > 1 && alphabet[0] == ',' && alphabet[1] != ',')
                throw new FormatException("Alphas list must start with a proper character");

            for (int i = 1; i < alphabet.Length; i += 2)
            {
                if (alphabet[i] != ',')
                    throw new FormatException("Comma seperated list contains a string and not a char");
            }

            if (alphabet[alphabet.Length - 1] == ',')
                throw new FormatException("alpha's list does not end in a character");
        }

        public void insertStates(string input)
        {
            input = input.Substring(8, input.Length - 9).TrimStart();

            string[] rawStates = input.Split(',');

            foreach(string x in rawStates)
            {
                states.Add(new State(x));
            }
        }

        public void insertStart(string input)
        {
            input = input.Substring(7, input.Length - 8).TrimStart();

            foreach(State x in states)
            {
                if (x.name.Equals(input))
                    startState = x;
            }
        }

        public void insertAccept(string input)
        {
            input = input.Substring(8, input.Length - 9).TrimStart();

            foreach (State x in states)
            {
                if (x.name.Equals(input))
                    acceptState = x;
            }
        }

        public void insertReject(string input)
        {
            input = input.Substring(8, input.Length - 9).TrimStart();

            foreach (State x in states)
            {
                if (x.name.Equals(input))
                    rejectState = x;
            }
        }

        public void insertAlpha(string input)
        {
            string alphabet = input.Substring(7, input.Length - 8).TrimStart();

            for(int i = 0; i < alphabet.Length; i+=2)
            {
                inputAlphabet += alphabet[i];
            }
            
            foreach(char x in inputAlphabet)
            {
                if (!(tapeAlphabet.Contains(x)))
                    throw new FormatException("Alphabet is not a subset of the tape alphabet");
            }
            
        }

        public void insertTapealpha(string input)
        {
            string alphabet = input.Substring(12, input.Length - 13).TrimStart();

            for (int i = 0; i < alphabet.Length; i += 2)
            {
                tapeAlphabet += alphabet[i];
            }

            tapeAlphabet += '_';
        }
    }

    class State
    {
        public List<Edge> edges { get; set; }
        public string name { get; set; }

        public State(string aname)
        {
            name = aname;
        }
    }

    class Edge
    {
        public char read { get; set; }
        public char write { get; set; }
        public State nextState { get; set; }
        public char moveHeader { get; set; }

        public Edge(char awrite, char aread, State anextState, char amoveHeader)
        {
            read = aread;
            write = awrite;
            nextState = anextState;
            moveHeader = amoveHeader;
        }
    }
}
