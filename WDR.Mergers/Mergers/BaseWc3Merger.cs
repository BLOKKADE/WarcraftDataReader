using WDR.Mappers.Models;

namespace WDR.Mergers.Mergers;

public abstract class BaseWc3DataMerger()
{
    protected void ExpandList(List<Wc3Object> targetList, List<Wc3Object> sourceList, Func<Wc3Object, string?> keySelector)
    {
        foreach (var sourceObject in sourceList)
        {
            var matchedObjects = targetList.Where(x => keySelector(x) == sourceObject.Code).ToList();
            if (matchedObjects.Count == 0)
            {
                if (sourceObject.Code != "-")
                {
                    targetList.Add(sourceObject);
                }
            }
            else
            {
                foreach (var targetObject in matchedObjects)
                {
                    Merge(targetObject, sourceObject);
                }
            }
        }
    }

    protected abstract void Merge(Wc3Object targetObject, Wc3Object sourceObject);
}
