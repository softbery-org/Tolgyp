// This file is part of Tolgyp, a C# library for language detection and translation.
/*
 * LICENSE MIT License
 * 
 * Copyright (c) 2025 Softbery by Paweł Tobis
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * Author						        : Paweł Tobis
 * Email							    : softbery@gmail.com
 * Description					    : 
 *                                           Key changes and clarifications:
 *                                           
 *                                           1. Class description (Language):
 *                                              - Highlights the main functionality: detection and translation.
 *                                              - Indicates the use of Google Cloud Translation V2 API.
 *                                              - Highlights the specificity of translation between Polish and English.
 *                                              - Added note about potential inefficiency related to re-creating TranslationClient on each 
 *                                              DetectLanguage(Async) call. 
 *                                              - In a more complex application it would be worth managing the client instance differently 
 *                                              (e.g. singleton, DI, lazy initialization in the field).
 *                                           2. Constructor description:
 *                                              - Standard description for the constructor.
 *                                              - Indicates that TranslationClient is initialized "on demand".
 *                                           3. Translate and TranslateAsync method descriptions:
 *                                              - Explains the logic of "switching" the language: if Polish is detected, translates to English; otherwise (for any 
 *                                              other detected language) translates to Polish.
 *                                              - Clarifies what is returned (translated text or error message).
 *                                              - In TranslateAsync, fixed return Task.FromResult<string>(translated.TranslatedText).Result; to return translated.
 *                                                TranslatedText;. Using. Result in an async method on a task that has just been awaited is redundant and can 
 *                                                lead to problems (although it would work in this particular case, it is bad practice). 
 *                                              - It is noted that these methods rely on a prior call to DetectLanguage(Async) to initialize _client and _detection. 
 *                                            4. The DetectLanguage and DetectLanguageAsync method descriptions: 
 *                                              - Explain that these methods initialize the _client field and set the detection result to the _detection field. 
 *                                              - They mention exception handling and logging to a file/console. 
 *                                              - For the async method (DetectLanguageAsync), I corrected the return type in the comment to Task, since 
 *                                              the method is an async Task. 
 *                                            5. Additional using: 
 *                                              - I added using System;, using System.IO;, using System.Threading.Tasks; for completeness, since your code 
 *                                              uses Console, File, and Task. 
 *                                              
 *                                           These descriptions should be helpful to anyone using this class or trying to understand how it works.
 * Create						        : 2023-03-12 04:31:42
 * Last Modification Date    : 2025-05-05 19:34:04
 */

using Google.Cloud.Translation.V2;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Tolgyp.lang
{
    /// <summary>
    /// Provides functionality to detect the language of a given text and translate it,
    /// primarily between Polish and English, using the Google Cloud Translation V2 API.
    /// It includes both synchronous and asynchronous methods for these operations.
    /// Note: The <see cref="TranslationClient"/> is currently re-initialized on each detection call,
    /// which might be inefficient for frequent operations.
    /// </summary>
    public class Language
    {
        private TranslationClient? _client;
        private Detection? _detection;
        private string? _languageCode = "pl";
        private string? _languageTranslateCode = "en";
        private string? _orginalText = string.Empty;
        private string? _translatedText = string.Empty;

        /// <summary>
        /// Gets or sets the language code for the detected language.
        /// </summary>
        /// <value>Default is set \"pl\"</value>
        public string? LanguageCode
        { 
            get=>_languageCode; // Property to expose the detected language code
            set=> _languageCode = value;
        }
        /// <summary>
        /// Gets or sets the language code for the translation.
        /// </summary>
        public string? LanguageTranslateCode
        {
            get => _languageTranslateCode; // Property to expose the translation language code
            set => _languageTranslateCode = value;
        }
        /// <summary>
        /// Gets the detected language and its confidence.
        /// </summary>
        public Detection? Detection => _detection; // Property to expose the detection result
        /// <summary>
        /// Gets the translation client.
        /// </summary>
        public TranslationClient? Client => _client; // Property to expose the client
        /// <summary>
        /// Gets the original text and translated text.
        /// </summary>
        public string? OrginalText { get => _orginalText; } // Property to store the original text
        /// <summary>
        /// Gets the translated text.
        /// </summary>
        public string? TranslatedText { get => _translatedText; } // Property to store the translated text

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class.
        /// The <see cref="TranslationClient"/> is initialized on demand when detection methods are called.
        /// </summary>
        public Language() {}

        /// <summary>
        /// Detects the language of the input text and translates it to the complementary language
        /// (Polish to English, or any other detected language to Polish).
        /// If the detected language is "pl", it translates to "en". Otherwise, it translates to "pl".
        /// </summary>
        /// <param name="argument">The text to be translated.</param>
        /// <returns>The translated text. Returns an error message string if detection or client is null, or translation fails.</returns>
        public string Translate(string argument)
        {
            DetectLanguage(argument); // This initializes _client and _detection
            TranslationResult? translated = null;

            if (_detection != null && _client != null)
            {
                // If detected language is Polish, target is English; otherwise, target is Polish.
                var lang = _detection.Language == _languageCode ? _languageTranslateCode : _languageCode;
                
                translated = _client.TranslateText(argument, lang);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n[Tolgyp] [Result]: Translated:\n{translated.TranslatedText}");
                Console.ForegroundColor = ConsoleColor.White;

                _orginalText = translated.OriginalText;
                _translatedText = translated.TranslatedText;

                return translated.TranslatedText;
            }
            // This message will be returned if DetectLanguage failed to initialize _client or _detection (e.g., due to an exception)
            return new Exception("[Tolgyp] [Error]: Can't translate because detection or client is null.").Message;
        }

        /// <summary>
        /// Asynchronously detects the language of the input text and translates it to the complementary language
        /// (Polish to English, or any other detected language to Polish).
        /// If the detected language is "pl", it translates to "en". Otherwise, it translates to "pl".
        /// </summary>
        /// <param name="argument">The text to be translated.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the translated text.
        /// Returns an error message string if detection or client is null, or translation fails.
        /// </returns>
        public async Task<string> TranslateAsync(string argument)
        {
            await DetectLanguageAsync(argument); // This initializes _client and _detection

            TranslationResult? translated = null;
            
            if (_detection != null && _client != null)
            {
                // If detected language is Polish, target is English; otherwise, target is Polish.
                var lang = _detection.Language == _languageCode ? _languageTranslateCode : _languageCode;

                translated = await _client.TranslateTextAsync(argument, lang);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n[Tolgyp]: Translated:\n{translated.TranslatedText}");
                Console.ForegroundColor = ConsoleColor.White;

                _orginalText = translated.OriginalText;
                _translatedText = translated.TranslatedText;

                // No need for Task.FromResult(...).Result in an async method.
                // The result of an awaited task is directly its value.
                return translated.TranslatedText;
            }
            // This message will be returned if DetectLanguageAsync failed to initialize _client or _detection
            return "[Tolgyp]: Can't translate because detection or client is null.";
        }

        /// <summary>
        /// Detects the language of the provided text using the Google Cloud Translation V2 API.
        /// The result of the detection (language code and confidence) is stored internally in the <see cref="_detection"/> field.
        /// This method also initializes the <see cref="_client"/> field with a new <see cref="TranslationClient"/>.
        /// Any exceptions during detection are caught, logged to "error.log", and printed to the console.
        /// </summary>
        /// <param name="argument">The text whose language needs to be detected.</param>
        public void DetectLanguage(string argument)
        {
            try
            {
                _client = TranslationClient.Create();
                _detection = _client.DetectLanguage(argument);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine($"[Tolgyp]: Language: {_detection.Language}; confidence {_detection.Confidence}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                File.WriteAllText("error.log", ex.ToString()); // Consider appending or more robust logging
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"[Tolgyp]: Error during language detection: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                // _client and _detection might be null or in an inconsistent state here
            }
        }

        /// <summary>
        /// Asynchronously detects the language of the provided text using the Google Cloud Translation V2 API.
        /// The result of the detection (language code and confidence) is stored internally in the <see cref="_detection"/> field.
        /// This method also initializes the <see cref="_client"/> field with a new <see cref="TranslationClient"/> asynchronously.
        /// Any exceptions during detection are caught, logged to "error.log", and printed to the console.
        /// </summary>
        /// <param name="argument">The text whose language needs to be detected.</param>
        /// <returns>A task that represents the asynchronous detection operation.</returns>
        public async Task DetectLanguageAsync(string argument)
        {
            try
            {
                _client = await TranslationClient.CreateAsync();
                _detection = await _client.DetectLanguageAsync(argument);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine($"[Tolgyp]: Language: {_detection.Language}; confidence {_detection.Confidence}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                File.WriteAllText("error.log", ex.ToString()); // Consider appending or more robust logging
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"[Tolgyp]: Error during asynchronous language detection: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                // _client and _detection might be null or in an inconsistent state here
            }
        }
    }
}