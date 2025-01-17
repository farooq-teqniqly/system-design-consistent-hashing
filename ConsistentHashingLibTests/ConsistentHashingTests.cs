using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using ConsistentHashingLib;
using Xunit.Abstractions;

namespace ConsistentHashingLibTests;

public class ConsistentHashingTests
{
    private readonly ITestOutputHelper _output;

    public ConsistentHashingTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Should_Add_Nodes()
    {
        var hashing = new ConsistentHashing<string>(1000);

        hashing.AddNode("A");
        hashing.AddNode("B");
        hashing.AddNode("C");
        hashing.AddNode("D");
        hashing.AddNode("E");
        hashing.AddNode("F");
        hashing.AddNode("G");
        hashing.AddNode("H");

        var faker = new Faker();
        var numKeysToGenerate = 100_000;
        var keys = faker.Lorem.Words(numKeysToGenerate);
        var histogram = new Dictionary<string, int>();

        foreach (var key in keys)
        {
            var assignedNode = hashing.GetNode(key);

            if (!histogram.TryAdd(assignedNode, 1))
            {
                histogram[assignedNode] += 1;
            }
        }
    }

}
