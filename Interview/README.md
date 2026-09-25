# 算法题说明

实现位于 `AlgorithmSolutions.cs`。

## 1. 检查括号是否匹配

使用栈保存左括号；遇到右括号时检查栈顶是否为对应类型。遍历结束后栈必须为空。

示例：

```csharp
AlgorithmSolutions.AreBracketsValid("()[{}]"); // true
AlgorithmSolutions.AreBracketsValid("([)]");   // false
AlgorithmSolutions.AreBracketsValid("");       // true
```

## 2. 合并两个有序数组

使用双指针分别遍历两个数组，每次把较小值写入结果数组。

示例：

```csharp
int[] result = AlgorithmSolutions.MergeSortedArrays(
    new[] { 1, 5, 7, 9 },
    new[] { 3, 6, 7, 10 });

// 结果：1, 3, 5, 6, 7, 7, 9, 10
```
