﻿using System.Text;
using QIQI.EProjectFile.Expressions;
namespace QIQI.EProjectFile.Statements
{
    /// <summary>
    /// 如果 语句块
    /// </summary>
    public class IfElseStatement : Statement
    {
        public Expression Condition { get; set; }
        /// <summary>
        /// <see cref="UnexaminedCode"/>不为null时，<see cref="Condition"/>应为null
        /// </summary>
        public string UnexaminedCode { get; set; }
        public StatementBlock BlockOnTrue { get; set; }
        public StatementBlock BlockOnFalse { get; set; }
        public string Comment { get; set; }
        public bool Mask { get; set; }

        public override void ToTextCode(IdToNameMap nameMap, StringBuilder result, int indent = 0)
        {
            for (int i = 0; i < indent; i++)
                result.Append("    ");
            if (Mask)
                result.Append("' ");
            if (UnexaminedCode == null)
            {
                result.Append(".如果 (");
                Condition.ToTextCode(nameMap, result, indent);
                result.Append(")");
            }
            else
            {
                result.Append(".");
                result.Append(UnexaminedCode);
            }
            if (Comment != null)
            {
                result.Append("  ' ");
                result.Append(Comment);
            }
            result.AppendLine();
            BlockOnTrue.ToTextCode(nameMap, result, indent + 1);
            result.AppendLine();
            for (int i = 0; i < indent; i++)
                result.Append("    ");
            result.AppendLine(".否则");
            BlockOnFalse.ToTextCode(nameMap, result, indent + 1);
            result.AppendLine();
            for (int i = 0; i < indent; i++)
                result.Append("    ");
            result.Append(".如果结束");
        }
        internal override void WriteTo(MethodCodeDataWriterArgs a)
        {
            using (a.NewBlock(1))
            {
                if (UnexaminedCode != null)
                    new UnexaminedStatement(UnexaminedCode, Mask).WriteTo(a, 0x6B);
                else
                    new ExpressionStatement(new CallExpression(0, 0, new ParamListExpression() { Condition }), Mask, Comment).WriteTo(a, 0x6B);
                BlockOnTrue.WriteTo(a);
                a.ExpressionData.Write((byte)0x50);
                BlockOnFalse.WriteTo(a);
                a.ExpressionData.Write((byte)0x51);
            }
            a.ExpressionData.Write((byte)0x72);
        }
    }
}
