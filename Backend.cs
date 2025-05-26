using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetGeminiSDK;
using DotnetGeminiSDK.Client;
using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Config;
using Microsoft.Extensions.DependencyInjection;


namespace POE_Part_2
{
    

    internal class Backend
    {
        private String question;
        

        private readonly GeminiClient _geminiClient;

        public Backend(string question)
        {
            // Initialize the GeminiClient with the default configuration
            _geminiClient = new GeminiClient(new GoogleGeminiConfig() 
            {
               ApiKey = "AIzaSyAy8kdkXnByuw_g3_7du3wDs9ndNuk9ow4", TextBaseUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash"

            });
            this.question = question;            
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
                            Console.WriteLine(textPart.Text);
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
