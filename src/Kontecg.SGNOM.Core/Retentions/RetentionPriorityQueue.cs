using System.Collections.Generic;

namespace Kontecg.Retentions
{
    public class RetentionPriorityQueue : PriorityQueue<RetentionDocument, int>
    {
        public void Enqueue(RetentionDocument document)
        {
            if(document is {RetentionDefinition: not null})
                Enqueue(document, document.RetentionDefinition.Priority);
        }
    }
}