using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelSamples.Samples.QuestionGenerationAgentSamples;
internal class Question
{
    public required string Text { get; set; }

    public required Answer[] Answers { get; set; }

    public char CorrectAnswer { get; set; }    
}

internal class Answer
{
    public required char Choice { get; set; }
    public required string Text { get; set; }    
}
