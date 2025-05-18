The program is named Tolgyp and is a console application written in C#. Its main task is to detect the language of the provided text and translate it.
The program uses the Google Cloud Translation API version V2 to perform language operations.
The specific translation logic in this program involves automatically switching between Polish (pl) and English (en):
 * If the program detects that the input text is in Polish, it will translate it into English.
 * If the program detects that the text is in any other language (by default assumed to be a language to be translated into Polish), it will translate it into Polish.
   The program supports command-line arguments:
 * It accepts the text to be translated as one of the arguments (usually the last one that is not an option).
 * Options such as -h or --help display help.
 * Options -v or --version display program version information.
 * The --detect-lang option only detects the language of the provided text without translating it.
   The Language class contains the logic for communicating with the Google API, including synchronous (DetectLanguage, Translate) and asynchronous (DetectLanguageAsync, TranslateAsync) methods. The translation process is always preceded by language detection. The detection results (language and confidence) and the translated text are displayed on the console.
   The program includes basic error handling: in case of problems during communication with the API (e.g., detection error), exceptions are caught, logged to the "error.log" file, and displayed on the console.
   The code is provided under the MIT license. Comments in the code indicate awareness of potential inefficiency related to creating a new instance of the API client with each detection operation.
   In summary, Tolgyp is a simple console tool for automatic text translation between Polish and English using the Google Cloud Translation API, with the ability for language detection only and basic error handling.
