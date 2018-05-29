using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using paintedNumbers;

public class paintedNumbersScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] numbers;
    public GameObject[] numbersRend;
    public TextMesh[] numbersText;
    public Renderer[] bursts;
    public Texture[] burstColors;

    //Get Numbers
    public Color[] colorOptions;
    public String[] colorNames;
    private List<int> selectedNumbers = new List<int>();
    private List<int> selectedColors = new List<int>();
    private int choice = 0;
    private string choiceText = "";
    private int colorChoice = 0;
    public List<int> colorCount = new List<int>();

    //Variables
    public List<int> primes = new List<int>();
    private List<int> cornerColors = new List<int>();
    private int buttonPresses = 0;
    private int correctAnswerLength = 0;
    private int givenAnswer = 0;

    //Rules
    private int rulesCounted = 0;
    private bool backwards = false;
    private string correctAnswer = "";
    private int pickedNumber = 0;
    private string answer = "";

    //Logging
    private bool moduleSolved = false;
    static int moduleIdCounter = 1;
    int moduleId;

    void Update()
    {
        if (moduleSolved)
        {

        }
        else if (answer == correctAnswer)
        {
            moduleSolved = true;
            Audio.PlaySoundAtTransform("spray", transform);
            numbers[0].AddInteractionPunch(.5f);
            Debug.LogFormat("[Graffiti Numbers #{0}] You have sprayed {1}. That is correct. Module disarmed.", moduleId, answer);
            answer = "";
            GetComponent<KMBombModule>().HandlePass();
        }
        else if (answer.Length == correctAnswer.Length)
        {
            Debug.LogFormat("[Graffiti Numbers #{0}] Strike! You have sprayed {1}. That is incorrect.", moduleId, answer);
            answer = "";
            givenAnswer = 0;
            foreach (Renderer burst in bursts)
            {
                burst.enabled = false;
            }
            foreach (GameObject number in numbersRend)
            {
                number.SetActive(true);
            }
            GetComponent<KMBombModule>().HandleStrike();
        }
    }

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable number in numbers)
        {
            KMSelectable trueNumber = number;
            number.OnInteract += delegate () { numberPress(trueNumber); return false; };
        }
    }

    void Start()
    {
        foreach (Renderer burst in bursts)
        {
            burst.enabled = false;
        }
        GetNumbers();
        GetAnswer();
        Debug.LogFormat("[Graffiti Numbers #{0}] The correct answer is {1}.", moduleId, correctAnswer);
        correctAnswerLength = correctAnswer.Length;
    }

    void GetNumbers()
    {
        foreach (TextMesh number in numbersText)
        {
            choice = UnityEngine.Random.Range(1,10);
            while (selectedNumbers.Contains(choice))
            {
                choice = UnityEngine.Random.Range(1,10);
            }
            selectedNumbers.Add(choice);
            choiceText = choice.ToString();
            number.text = choiceText;
            colorChoice = UnityEngine.Random.Range(0,4);
            number.color = colorOptions[colorChoice];
            selectedColors.Add(colorChoice);
            colorCount[colorChoice]++;
        }
        Debug.LogFormat("[Graffiti Numbers #{0}] The top row is {1} {2}, {3} {4}, {5} {6}.", moduleId, colorNames[selectedColors[0]], selectedNumbers[0], colorNames[selectedColors[1]], selectedNumbers[1], colorNames[selectedColors[2]], selectedNumbers[2]);
        Debug.LogFormat("[Graffiti Numbers #{0}] The middle row is {1} {2}, {3} {4}, {5} {6}.", moduleId, colorNames[selectedColors[3]], selectedNumbers[3], colorNames[selectedColors[4]], selectedNumbers[4], colorNames[selectedColors[5]], selectedNumbers[5]);
        Debug.LogFormat("[Graffiti Numbers #{0}] The bottom row is {1} {2}, {3} {4}, {5} {6}.", moduleId, colorNames[selectedColors[6]], selectedNumbers[6], colorNames[selectedColors[7]], selectedNumbers[7], colorNames[selectedColors[8]], selectedNumbers[8]);
    }

    void GetAnswer()
    {
        if (colorCount[0] > colorCount[1])
        {
            Debug.LogFormat("[Graffiti Numbers #{0}] Start at rule 9 and work backwards.", moduleId);
            backwards = true;
            Rule8();
        }
        else if (colorCount[2] > colorCount[3])
        {
            Debug.LogFormat("[Graffiti Numbers #{0}] Start at rule 6 and work forwards.", moduleId);
            Rule6();
        }
        else if (colorCount[1] > colorCount[2])
        {
            Debug.LogFormat("[Graffiti Numbers #{0}] Start at rule 4 and work backwards.", moduleId);
            backwards = true;
            Rule4();
        }
        else
        {
            Debug.LogFormat("[Graffiti Numbers #{0}] Start at rule 1 and work forwards.", moduleId);
            Rule1();
        }
    }

    void Rule1()
    {
        if (selectedNumbers[0] < selectedNumbers[3] && selectedNumbers[3] < selectedNumbers[6])
        {
            correctAnswer += "1";
            buttonPresses += 1;
        }
        rulesCounted++;
        if (rulesCounted == 9)
        {
            return;
        }
        else if (backwards)
        {
            Rule9();
        }
        else
        {
            Rule2();
        }
    }

    void Rule2()
    {
        if (selectedNumbers[0] > selectedNumbers[1] && selectedNumbers[1] > selectedNumbers[2])
        {
            correctAnswer += "2";
            buttonPresses += 1;
        }
        rulesCounted++;
        if (rulesCounted == 9)
        {
            return;
        }
        else if (backwards)
        {
            Rule1();
        }
        else
        {
            Rule3();
        }
    }

    void Rule3()
    {
        if (selectedNumbers[0] + selectedNumbers[2] + selectedNumbers[6] + selectedNumbers [8] > selectedNumbers[1] + selectedNumbers[3] + selectedNumbers[4] + selectedNumbers[5] + selectedNumbers[7])
        {
            correctAnswer += "3";
            buttonPresses += 1;
        }
        rulesCounted++;
        if (rulesCounted == 9)
        {
            return;
        }
        else if (backwards)
        {
            Rule2();
        }
        else
        {
            Rule4();
        }
    }

    void Rule4()
    {
        foreach (TextMesh number in numbersText)
        {
            if (number.text == "1")
            {
                int index = System.Array.IndexOf (numbersText, number);
                if (selectedColors[index] == 2)
                {
                    correctAnswer += "4";
                    buttonPresses += 1;
                }
            }
        }
        rulesCounted++;
        if (rulesCounted == 9)
        {
            return;
        }
        else if (backwards)
        {
            Rule3();
        }
        else
        {
            Rule5();
        }
    }

    void Rule5()
    {
        if (primes.Contains(selectedNumbers[0]) && primes.Contains(selectedNumbers[1]) || primes.Contains(selectedNumbers[0]) && primes.Contains(selectedNumbers[2]) || primes.Contains(selectedNumbers[1]) && primes.Contains(selectedNumbers[2]))
        {
            correctAnswer += "5";
            buttonPresses += 1;
        }
        rulesCounted++;
        if (rulesCounted == 9)
        {
            return;
        }
        else if (backwards)
        {
            Rule4();
        }
        else
        {
            Rule6();
        }
    }

    void Rule6()
    {
        if ((selectedNumbers[0] + selectedNumbers[3]) % 10 == selectedNumbers[6] || (selectedNumbers[1] + selectedNumbers[4]) % 10 == selectedNumbers[7] || (selectedNumbers[2] + selectedNumbers[5]) % 10 == selectedNumbers[8])
        {
            correctAnswer += "6";
            buttonPresses += 1;
        }
        rulesCounted++;
        if (rulesCounted == 9)
        {
            return;
        }
        else if (backwards)
        {
            Rule5();
        }
        else
        {
            Rule7();
        }
    }

    void Rule7()
    {
        if (selectedNumbers[0] + selectedNumbers[8] != selectedNumbers[2] + selectedNumbers[6])
        {
            correctAnswer += "7";
            buttonPresses += 1;
        }
        rulesCounted++;
        if (rulesCounted == 9)
        {
            return;
        }
        else if (backwards)
        {
            Rule6();
        }
        else
        {
            Rule8();
        }
    }

    void Rule8()
    {
        cornerColors.Add(selectedColors[0]);
        cornerColors.Add(selectedColors[2]);
        cornerColors.Add(selectedColors[6]);
        cornerColors.Add(selectedColors[8]);
        if (cornerColors.Distinct().Count() <= 2)
        {
            correctAnswer += "8";
            buttonPresses += 1;
        }
        rulesCounted++;
        if (rulesCounted == 9)
        {
            return;
        }
        else if (backwards)
        {
            Rule7();
        }
        else
        {
            Rule9();
        }
    }

    void Rule9()
    {
        if (buttonPresses < 3)
        {
            correctAnswer += "9";
            buttonPresses += 1;
        }
        rulesCounted++;
        if (rulesCounted == 9)
        {
            return;
        }
        else if (backwards)
        {
            Rule8();
        }
        else
        {
            Rule1();
        }
    }

    public void numberPress(KMSelectable number)
    {

        if (moduleSolved)
        {
            return;
        }

        if (givenAnswer + 1 == correctAnswerLength)
        {

        }
        else
        {
            number.AddInteractionPunch(.5f);
            Audio.PlaySoundAtTransform("spray", transform);
        }

        if (number == numbers[0])
        {
            pickedNumber = selectedNumbers[0];
            answer += pickedNumber.ToString();
            bursts[0].enabled = true;
            numbersRend[0].SetActive(false);
            int burstText = UnityEngine.Random.Range(0,4);
            bursts[0].material.mainTexture = burstColors[burstText];
        }
        else if (number == numbers[1])
        {
            pickedNumber = selectedNumbers[1];
            answer += pickedNumber.ToString();
            bursts[1].enabled = true;
            numbersRend[1].SetActive(false);
            int burstText = UnityEngine.Random.Range(0,4);
            bursts[1].material.mainTexture = burstColors[burstText];
        }
        else if (number == numbers[2])
        {
            pickedNumber = selectedNumbers[2];
            answer += pickedNumber.ToString();
            bursts[2].enabled = true;
            numbersRend[2].SetActive(false);
            int burstText = UnityEngine.Random.Range(0,4);
            bursts[2].material.mainTexture = burstColors[burstText];
        }
        else if (number == numbers[3])
        {
            pickedNumber = selectedNumbers[3];
            answer += pickedNumber.ToString();
            bursts[3].enabled = true;
            numbersRend[3].SetActive(false);
            int burstText = UnityEngine.Random.Range(0,4);
            bursts[3].material.mainTexture = burstColors[burstText];
        }
        else if (number == numbers[4])
        {
            pickedNumber = selectedNumbers[4];
            answer += pickedNumber.ToString();
            bursts[4].enabled = true;
            numbersRend[4].SetActive(false);
            int burstText = UnityEngine.Random.Range(0,4);
            bursts[4].material.mainTexture = burstColors[burstText];
        }
        else if (number == numbers[5])
        {
            pickedNumber = selectedNumbers[5];
            answer += pickedNumber.ToString();
            bursts[5].enabled = true;
            numbersRend[5].SetActive(false);
            int burstText = UnityEngine.Random.Range(0,4);
            bursts[5].material.mainTexture = burstColors[burstText];
        }
        else if (number == numbers[6])
        {
            pickedNumber = selectedNumbers[6];
            answer += pickedNumber.ToString();
            bursts[6].enabled = true;
            numbersRend[6].SetActive(false);
            int burstText = UnityEngine.Random.Range(0,4);
            bursts[6].material.mainTexture = burstColors[burstText];
        }
        else if (number == numbers[7])
        {
            pickedNumber = selectedNumbers[7];
            answer += pickedNumber.ToString();
            bursts[7].enabled = true;
            numbersRend[7].SetActive(false);
            int burstText = UnityEngine.Random.Range(0,4);
            bursts[7].material.mainTexture = burstColors[burstText];
        }
        else if (number == numbers[8])
        {
            pickedNumber = selectedNumbers[8];
            answer += pickedNumber.ToString();
            bursts[8].enabled = true;
            numbersRend[8].SetActive(false);
            int burstText = UnityEngine.Random.Range(0,4);
            bursts[8].material.mainTexture = burstColors[burstText];
        }
        givenAnswer++;
    }

    #pragma warning disable 414
        private string TwitchHelpMessage = @"Use !{0} spray 1 2 5 6 8 to press the numbers POSITION in the order. (Reading order)";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        Debug.Log(command);
        var parts = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 0 && parts[0] == "spray" && parts.Skip(1).All(part => part.Length == 1 && "123456789".Contains(part)))
        {
            yield return null;

            var cmdNumbers = parts.Skip(1).ToArray();

            for (int i = 0; i < cmdNumbers.Length; i++)
            {
                int num;
                int.TryParse(cmdNumbers[i], out num);

                numberPress(numbers[num - 1]);

                yield return new WaitForSeconds(.2f);
            }
        }
    }
}
