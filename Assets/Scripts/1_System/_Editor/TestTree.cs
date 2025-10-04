using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class NamespaceTreeGenerator
{
    private class Node
    {
        public string Name;
        public string Type; // namespace, class, struct, enum, interface
        public List<Node> Children = new List<Node>();
    }

    public static void GenerateNamespaceTree(string rootNamespace, string outputPath, int indentSpaces = 2)
    {
        string[] csFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        var rootNode = new Node { Name = rootNamespace, Type = "namespace" };

        foreach (string file in csFiles)
        {
            string code = File.ReadAllText(file);
            ExtractNamespace(rootNode, code, rootNamespace);
        }

        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            WriteTree(writer, rootNode, "", true, indentSpaces);
        }

        Debug.Log($"Namespace tree written to: {outputPath}");
    }

    private static void ExtractNamespace(Node parentNode, string code, string targetNamespace)
    {
        var nsPattern = new Regex(@"\bnamespace\s+([\w\.]+)\s*{", RegexOptions.Singleline);
        foreach (Match match in nsPattern.Matches(code))
        {
            string fullNamespace = match.Groups[1].Value;
            if (!fullNamespace.StartsWith(targetNamespace)) continue;

            int bodyStart = match.Index + match.Length;
            int bodyEnd = FindMatchingBrace(code, bodyStart - 1);
            if (bodyEnd == -1) continue;

            string innerCode = code.Substring(bodyStart, bodyEnd - bodyStart);
            var nsNode = FindOrCreateChild(parentNode, fullNamespace, "namespace");
            ParseTypes(nsNode, innerCode);
        }
    }

    private static void ParseTypes(Node parentNode, string code)
    {
        var typePattern = new Regex(@"\b(class|struct|enum|interface)\s+(\w+)\s*(<[^>]+>)?\s*(?:[:{])", RegexOptions.Singleline);

        for (int i = 0; i < code.Length;)
        {
            Match match = typePattern.Match(code, i);
            if (!match.Success) break;

            string type = match.Groups[1].Value;
            string name = match.Groups[2].Value;
            int bodyStart = match.Index + match.Length;
            int bodyEnd = FindMatchingBrace(code, bodyStart - 1);
            if (bodyEnd == -1) break;

            string body = code.Substring(bodyStart, bodyEnd - bodyStart);
            var node = new Node { Name = name, Type = type };
            parentNode.Children.Add(node);
            ParseTypes(node, body); // 再帰的にネスト型解析
            i = bodyEnd + 1;
        }
    }

    private static int FindMatchingBrace(string code, int startIndex)
    {
        int depth = 0;
        for (int i = startIndex; i < code.Length; i++)
        {
            if (code[i] == '{') depth++;
            else if (code[i] == '}')
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        return -1;
    }

    private static Node FindOrCreateChild(Node root, string fullNamespace, string type)
    {
        string[] parts = fullNamespace.Split('.');
        Node current = root;
        foreach (string part in parts)
        {
            Node child = current.Children.Find(n => n.Type == "namespace" && n.Name == part);
            if (child == null)
            {
                child = new Node { Name = part, Type = "namespace" };
                current.Children.Add(child);
            }
            current = child;
        }
        return current;
    }

    private static void WriteTree(StreamWriter writer, Node node, string prefix, bool isLast, int indentSpaces)
    {
        string indent = new string(' ', indentSpaces);
        string connector = isLast ? "└" : "├";
        string branch = $"{prefix}{connector}── {node.Type}: {node.Name}";
        writer.WriteLine(branch);

        string childPrefix = prefix + (isLast ? "   " : "│" + indent);
        for (int i = 0; i < node.Children.Count; i++)
        {
            bool last = (i == node.Children.Count - 1);
            WriteTree(writer, node.Children[i], childPrefix, last, indentSpaces);
        }
    }
}


public class TestTree: MonoBehaviour
{
    [Button("TreeWrite")]
    public bool button0;

    public string rootNamespace;

    public void TreeWrite()
    {
        string outputPath = Application.dataPath + "/NamespaceTree.txt";
        NamespaceTreeGenerator.GenerateNamespaceTree(rootNamespace, outputPath,2);
    }
}
