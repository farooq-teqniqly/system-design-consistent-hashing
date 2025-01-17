using System.Security.Cryptography;
using System.Text;

namespace ConsistentHashingLib;

public class ConsistentHashing<T>
{
    private readonly SortedDictionary<int, T> _hashRing = [];
    private readonly int _virtualNodeCount;                     
    private readonly Func<string, int> _hashFunction;        

    public ConsistentHashing(int virtualNodeCount = 100, Func<string, int>? hashFunction = null)
    {
        _virtualNodeCount = virtualNodeCount;
        _hashFunction = hashFunction ?? DefaultHashFunction;
    }

    private static int DefaultHashFunction(string input)
    {
        var hash = SHA1.HashData(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToInt32(hash, 0) & 0x7FFFFFFF; // Ensure non-negative values
    }

    public void AddNode(T physicalNode)
    {
        for (var i = 0; i < _virtualNodeCount; i++)
        {
            var virtualNodeKey = $"{physicalNode}-VN-{i}";
            var hash = _hashFunction(virtualNodeKey);
            _hashRing[hash] = physicalNode;
        }
    }

    public void RemoveNode(T physicalNode)
    {
        for (var i = 0; i < _virtualNodeCount; i++)
        {
            var virtualNodeKey = $"{physicalNode}-VN-{i}";
            var hash = _hashFunction(virtualNodeKey);
            _hashRing.Remove(hash);
        }
    }

    public T GetNode(string key)
    {
        if (_hashRing.Count == 0)
        {
            throw new InvalidOperationException("Hash ring is empty. Add nodes before querying.");
        }

        var hash = _hashFunction(key);
        var matchingNode = _hashRing.FirstOrDefault(kvp => kvp.Key >= hash);

        return matchingNode.Value ?? _hashRing.First().Value;
    }

    public IEnumerable<T> GetAllNodes()
    {
        return _hashRing.Values.Distinct();
    }
}
