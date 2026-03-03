using StilettoSQL.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StilettoSQL.Internal;

internal partial class PreprocessSQL {
    // Паттерн:
    // '               - открывающая кавычка
    // (               - начало группы содержимого строки
    //   \\.           - ЛИБО любой символ после бэкслеша (экранирование \' или \n)
    //   |             - ИЛИ
    //   ''            - две кавычки подряд (стандарт SQL)
    //   |             - ИЛИ
    //   [^']          - любой символ, кроме кавычки
    // )*              - повторяем 0 и более раз
    // '               - закрывающая кавычка
    // |(\?\?)         - ИЛИ наши знаки вопроса (группа 1)
    [GeneratedRegex(@"'(\\.|''|[^'])*'|(\?\?)", RegexOptions.Compiled)]
    private static partial Regex SqlPlaceholderWithEscapingRegex();

    public static string ReplacePlaceholders(string sql, int counter = 0) {
        return SqlPlaceholderWithEscapingRegex().Replace(sql, m => {
            // Проверяем именно Группу 1 (вторая часть регулярки)
            if (m.Groups[1].Success) {
                return $"${++counter}";
            }
            return m.Value;
        });
    }

}


