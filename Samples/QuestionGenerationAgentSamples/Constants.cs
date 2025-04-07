﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelSamples.Samples.QuestionGenerationAgentSamples;
internal static class Constants
{
    public const string GenerationAgent = "Generator";
    public const string GenerationInstructions = """
        You're job is to generate a question and corresponding answer choices based on the user input. 
        You will be provided with a user input and you need to generate a question based on that input. 
        The question should be clear and concise, and the answer choices should be relevant to the question. 
        Please make sure to provide at least 4 answer choices, only one of them should be the correct answer. 
        The other choices should be plausible but incorrect.
    """;

    public const string EvaluationAgent = "Evaluator";
    public const string EvaluationInstructions = """
        You're job is to evaluate the question and answer choices generated by the Question Generation Agent.
        Please provide feedback on the question and answer choices, and suggest areas for improvement.
        Consider the question in isolation, the question must make sense on its own and not depend on the user input.
        There should only ever be one correct answer.
        Distractors should be relevant to the question while still being incorrect. 
        We don't want the distractors to be too obvious though.
        Do not provide the suggested updates yourself.
        At the end of your response, please provide an overall assessment and a scoring from 1-10 with 10 being the highest quality.
        Be very strict in your assessment and scoring, we only want the highest quality questions and answers.
    """;

    public const string ImprovementAgent = "Improver";
    public const string ImprovementInstructions = """
        You're job is to act on the feedback from the evaluator.
        You will be provided clear feedback from the evaluator and you need to improve the question and answer choices based on that feedback.
        Please make sure to address all the feedback and provide a revised version of the question and answer choices.
        Do not provide any new feedback or suggestions, just focus on improving the question and answer choices.
    """;
}
