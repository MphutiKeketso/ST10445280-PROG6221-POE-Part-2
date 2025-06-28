using DotnetGeminiSDK;
using DotnetGeminiSDK.Client;
using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Config;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace POE_Part_2
{


    internal class Backend
    {
        private String question;
        private String answer = "";
        private String referenceQuestion;
        private readonly GeminiClient _geminiClient;

        public Backend(string question)
        {
            // Initialize the GeminiClient with the default configuration
            _geminiClient = new GeminiClient(new GoogleGeminiConfig()
            {
                ApiKey = "AIzaSyDkBITrXmRurpucCgG1jNA_7CXCA9UsM8g",
                TextBaseUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash"

            });
            this.question = question;
        }


        public void CheckQuiz()
        {
            // Ensure the question is not null or empty
            if (string.IsNullOrWhiteSpace(question))
            {
                Console.WriteLine("Question cannot be null or empty.");
                return;
            }
            else
            {
                String reference;
                reference = question;
                question = "Does the following question/statement contain the words 'quiz', 'task' or 'reminder'? If not please respond to the statement or question but do not mention that I asked if the statement/question contained the word 'quiz', but if it does contain the word 'quiz' then please answer using the word 'yes', no other words. If it contains the word 'task' then please answer using the word 'no', no other words. And If it contains the word 'reminder' then please answer using the word 'true', no other words. " + ":\n" + question;

                // Await the asynchronous Question method
                Task.Run(async () => await Question()).Wait();

                if (answer.ToLower() == "yes")
                {
                    Backend quiz = new Backend("");
                    quiz.Quiz(); // Call the Quiz method to start the quiz
                }
                else if (answer.ToLower() == "no")
                {
                    String desciption;
                    Console.WriteLine("Sure, please decribe the task you want me to add?");
                    desciption = Console.ReadLine();
                    Console.WriteLine("Thank you for the description. I will " + reference + " with the description: " + desciption);
                }
                else if (answer.ToLower() == "true")
                {
                    Console.WriteLine("Sure, I will " + reference);
                }
                else
                {
                    Console.WriteLine(answer);
                }
            }
        }


        public void Quiz()
        {
            // Ensure the question is not null or empty
            int score = 0; 
            Console.WriteLine("Welcome to the quiz! You will be asked 10 questions. Please answer each question. Your total will be showed at the end of the quiz\n");
            Backend quiz = new Backend("");
            for (int i = 0; i < 10; i++)
            {
                 // Example question
                quiz.questionGeneration(); // Generate a question               
                question = Console.ReadLine();

                if (question == "quit")
                {
                   Console.WriteLine("You have chosen to quit the quiz. Your score is: " + score);
                    Frontend gui2 = new Frontend(Program.name);
                    gui2.GUIManiuplation();
                    break;
                   
                }

                question = question + "\n This is my answer to the random easy cyber-security question that I asked you to generate, is it correct? If it is correct please respond with 'yes', if not, please provide a brief explaination with the correct answer. For reference, the question you asked me is: " +referenceQuestion;
                Task.Run(async () => await Question()).Wait();
                if (answer.ToLower() == "yes")
                {
                    score++;
                    Console.WriteLine("Correct! Your score is now: " + score);
                }
                else
                {
                   Console.WriteLine("Incorrect. " + answer);
                 
                }

            }

            String finalScore = "Your final score is: " + score + "/10";

            if (score >= 7)
            {
                finalScore += " - Well done!";
            }
            else if (score >= 4)
            {
                finalScore += " - Good effort!";
            }
            else
            {
                finalScore += " - Better luck next time!";
            }

            Console.WriteLine(finalScore);
        }

        public void questionGeneration()
        {
            Random random = new Random();
            int questionType = random.Next(0, 3);
            string additionalInfo = "";
            if (questionType == 0)
            {
                additionalInfo = "";
            }
            else if (questionType == 1)
            {
                additionalInfo = "and please make it a true or false question";
            }
            else if (questionType == 2)
            {
                additionalInfo = "and please make it a multiple choice question (four choices)";
            }
            else
            Console.WriteLine("JUST BEFORE ASKING GEMINI TO GENERATE THE QUESTION");
            question = "Please generate a random easy cyber-security question for me to answer " + additionalInfo ;
            Task.Run(async () => await Question()).Wait();
            Console.WriteLine(answer);
            referenceQuestion = answer; // Store the original question for reference
        }

        public async Task Question()
        {


            // Use the StreamTextPrompt method correctly without assigning its return value to a string

            try
            {
                
                var response = await _geminiClient.TextPrompt(question);
                EraseLine();
                if (response != null && response.Candidates != null && response.Candidates.Any())
                {
                    // Get the first candidate (usually the primary response)
                    var firstCandidate = response.Candidates.First();

                    // Each candidate has 'Content', and Content has 'Parts'.
                    // Each part can contain text, image data, etc. For text, we look for 'text'.
                    if (firstCandidate.Content != null && firstCandidate.Content.Parts != null && firstCandidate.Content.Parts.Any())
                    {
                        // Get the first part (assuming it's text for a simple TextPrompt)
                        var textPart = firstCandidate.Content.Parts.First();

                        // Check if the part actually contains text
                        if (!string.IsNullOrEmpty(textPart.Text)) // Ensure Text property exists and is not empty
                        {
                            // Console.WriteLine($"\nGemini response ({modelToUse}):");
                            answer = textPart.Text; // Trim whitespace
                            
                        }
                        else
                        {
                            Console.WriteLine("Gemini generated a response, but the text part was empty.");
                            // This can happen if, for example, the response is blocked for safety reasons
                            // or if the response is an image/tool_code etc.
                        }
                    }
                    else
                    {
                        Console.WriteLine("Gemini candidate has no content parts.");
                    }
                }
                else
                {
                    Console.WriteLine("No candidates were generated or the response was null.");
                    // This might indicate a safety filter blockage or another issue.
                    // You might check response.PromptFeedback for more details if available.
                }
                // Process the successful response
            }
            catch (HttpRequestException ex) // Catch HTTP related errors
            {
                Console.WriteLine($"HTTP Request Error: {ex.Message}");
                Console.WriteLine($"Status Code: {ex.StatusCode}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                // Try to get the actual response content from the API if available
                if (ex.Data.Contains("ResponseContent")) // Some HTTP client wrappers might put this in Data
                {
                    Console.WriteLine($"API Response Content: {ex.Data["ResponseContent"]}");
                }
            }
            catch (Exception ex) // Catch all other exceptions
            {
                Console.WriteLine($"An unexpected error occurred: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

            // No assignment to a string is needed as StreamTextPrompt returns void
            // Print the response directly
           
        }

        public static void EraseLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            int currentLineCursorTop = Console.CursorTop;
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursorTop);
            Console.WriteLine(); // Move to the next line after erasing
        }


        
    }
}
