using System;
using System.Collections.Generic;

namespace Interview
{
    /// <summary>
    /// Unity 工程师笔试中的两道算法题。
    /// </summary>
    public static class AlgorithmSolutions
    {
        /// <summary>
        /// 检查只包含 ()、{}、[] 的字符串是否合法。空字符串视为合法。
        /// 时间复杂度 O(n)，空间复杂度 O(n)。
        /// </summary>
        public static bool AreBracketsValid(string input)
        {
            if (input == null)
            {
                return false;
            }

            var openings = new Stack<char>();
            foreach (char character in input)
            {
                switch (character)
                {
                    case '(':
                    case '{':
                    case '[':
                        openings.Push(character);
                        break;
                    case ')':
                        if (!TryMatch(openings, '(')) return false;
                        break;
                    case '}':
                        if (!TryMatch(openings, '{')) return false;
                        break;
                    case ']':
                        if (!TryMatch(openings, '[')) return false;
                        break;
                    default:
                        return false;
                }
            }

            return openings.Count == 0;
        }

        /// <summary>
        /// 合并两个递增有序数组并返回新的递增数组。
        /// 时间复杂度 O(n + m)，空间复杂度 O(n + m)。
        /// </summary>
        public static int[] MergeSortedArrays(int[] nums1, int[] nums2)
        {
            if (nums1 == null) throw new ArgumentNullException(nameof(nums1));
            if (nums2 == null) throw new ArgumentNullException(nameof(nums2));

            var merged = new int[nums1.Length + nums2.Length];
            int index1 = 0;
            int index2 = 0;
            int mergedIndex = 0;

            while (index1 < nums1.Length && index2 < nums2.Length)
            {
                merged[mergedIndex++] = nums1[index1] <= nums2[index2]
                    ? nums1[index1++]
                    : nums2[index2++];
            }

            while (index1 < nums1.Length)
            {
                merged[mergedIndex++] = nums1[index1++];
            }

            while (index2 < nums2.Length)
            {
                merged[mergedIndex++] = nums2[index2++];
            }

            return merged;
        }

        private static bool TryMatch(Stack<char> openings, char expectedOpening)
        {
            return openings.Count > 0 && openings.Pop() == expectedOpening;
        }
    }
}
