using EcmaScript.NET.Collections;
using System;
using System.Runtime.InteropServices;
using System.Text;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class Node
	{
		internal class GetterPropertyLiteral
		{
			internal object Property;
			public GetterPropertyLiteral(object property)
			{
				this.Property = property;
			}
		}
		internal class SetterPropertyLiteral
		{
			internal object Property;
			public SetterPropertyLiteral(object property)
			{
				this.Property = property;
			}
		}
		private class NumberNode : Node
		{
			internal double number;
			internal NumberNode(double number) : base(39)
			{
				this.number = number;
			}
		}
		private class StringNode : Node
		{
			internal string str;
			internal StringNode(int Type, string str) : base(Type)
			{
				this.str = str;
			}
		}
		public class Jump : Node
		{
			public Node target;
			private Node target2;
			private Node.Jump jumpNode;
			public Node.Jump JumpStatement
			{
				get
				{
					if (this.Type != 118 && this.Type != 119)
					{
						Context.CodeBug();
					}
					return this.jumpNode;
				}
				set
				{
					if (this.Type != 118 && this.Type != 119)
					{
						Context.CodeBug();
					}
					if (value == null)
					{
						Context.CodeBug();
					}
					if (this.jumpNode != null)
					{
						Context.CodeBug();
					}
					this.jumpNode = value;
				}
			}
			public Node Default
			{
				get
				{
					if (this.Type != 112)
					{
						Context.CodeBug();
					}
					return this.target2;
				}
				set
				{
					if (this.Type != 112)
					{
						Context.CodeBug();
					}
					if (value.Type != 129)
					{
						Context.CodeBug();
					}
					if (this.target2 != null)
					{
						Context.CodeBug();
					}
					this.target2 = value;
				}
			}
			public Node Finally
			{
				get
				{
					if (this.Type != 79)
					{
						Context.CodeBug();
					}
					return this.target2;
				}
				set
				{
					if (this.Type != 79)
					{
						Context.CodeBug();
					}
					if (value.Type != 129)
					{
						Context.CodeBug();
					}
					if (this.target2 != null)
					{
						Context.CodeBug();
					}
					this.target2 = value;
				}
			}
			public Node.Jump Loop
			{
				get
				{
					if (this.Type != 128)
					{
						Context.CodeBug();
					}
					return this.jumpNode;
				}
				set
				{
					if (this.Type != 128)
					{
						Context.CodeBug();
					}
					if (value == null)
					{
						Context.CodeBug();
					}
					if (this.jumpNode != null)
					{
						Context.CodeBug();
					}
					this.jumpNode = value;
				}
			}
			public Node Continue
			{
				get
				{
					if (this.Type != 130)
					{
						Context.CodeBug();
					}
					return this.target2;
				}
				set
				{
					if (this.Type != 130)
					{
						Context.CodeBug();
					}
					if (value.Type != 129)
					{
						Context.CodeBug();
					}
					if (this.target2 != null)
					{
						Context.CodeBug();
					}
					this.target2 = value;
				}
			}
			public Jump(int Type) : base(Type)
			{
			}
			internal Jump(int Type, int lineno) : base(Type, lineno)
			{
			}
			internal Jump(int Type, Node child) : base(Type, child)
			{
			}
			internal Jump(int Type, Node child, int lineno) : base(Type, child, lineno)
			{
			}
		}
		private class PropListItem
		{
			internal Node.PropListItem next;
			internal int Type;
			internal int intValue;
			internal object objectValue;
		}
		public const int FUNCTION_PROP = 1;
		public const int LOCAL_PROP = 2;
		public const int LOCAL_BLOCK_PROP = 3;
		public const int REGEXP_PROP = 4;
		public const int CASEARRAY_PROP = 5;
		public const int TARGETBLOCK_PROP = 6;
		public const int VARIABLE_PROP = 7;
		public const int ISNUMBER_PROP = 8;
		public const int DIRECTCALL_PROP = 9;
		public const int SPECIALCALL_PROP = 10;
		public const int SKIP_INDEXES_PROP = 11;
		public const int OBJECT_IDS_PROP = 12;
		public const int INCRDECR_PROP = 13;
		public const int CATCH_SCOPE_PROP = 14;
		public const int LABEL_ID_PROP = 15;
		public const int MEMBER_TYPE_PROP = 16;
		public const int NAME_PROP = 17;
		public const int LAST_PROP = 17;
		public const int BOTH = 0;
		public const int LEFT = 1;
		public const int RIGHT = 2;
		public const int NON_SPECIALCALL = 0;
		public const int SPECIALCALL_EVAL = 1;
		public const int SPECIALCALL_WITH = 2;
		public const int DECR_FLAG = 1;
		public const int POST_FLAG = 2;
		public const int PROPERTY_FLAG = 1;
		public const int ATTRIBUTE_FLAG = 2;
		public const int DESCENDANTS_FLAG = 4;
		internal int Type;
		internal Node next;
		private Node first;
		private Node last;
		private int lineno = -1;
		private Node.PropListItem propListHead;
		public Node FirstChild
		{
			get
			{
				return this.first;
			}
		}
		public Node LastChild
		{
			get
			{
				return this.last;
			}
		}
		public Node Next
		{
			get
			{
				return this.next;
			}
		}
		public Node LastSibling
		{
			get
			{
				Node node = this;
				while (node.next != null)
				{
					node = node.next;
				}
				return node;
			}
		}
		public int Lineno
		{
			get
			{
				return this.lineno;
			}
		}
		public double Double
		{
			get
			{
				return ((Node.NumberNode)this).number;
			}
			set
			{
				((Node.NumberNode)this).number = value;
			}
		}
		public string String
		{
			get
			{
				return ((Node.StringNode)this).str;
			}
			set
			{
				if (value == null)
				{
					Context.CodeBug();
				}
				((Node.StringNode)this).str = value;
			}
		}
		public Node(int nodeType)
		{
			this.Type = nodeType;
		}
		public Node(int nodeType, Node child)
		{
			this.Type = nodeType;
			this.last = child;
			this.first = child;
			child.next = null;
		}
		public Node(int nodeType, Node left, Node right)
		{
			this.Type = nodeType;
			this.first = left;
			this.last = right;
			left.next = right;
			right.next = null;
		}
		public Node(int nodeType, Node left, Node mid, Node right)
		{
			this.Type = nodeType;
			this.first = left;
			this.last = right;
			left.next = mid;
			mid.next = right;
			right.next = null;
		}
		public Node(int nodeType, int line)
		{
			this.Type = nodeType;
			this.lineno = line;
		}
		public Node(int nodeType, Node child, int line) : this(nodeType, child)
		{
			this.lineno = line;
		}
		public Node(int nodeType, Node left, Node right, int line) : this(nodeType, left, right)
		{
			this.lineno = line;
		}
		public Node(int nodeType, Node left, Node mid, Node right, int line) : this(nodeType, left, mid, right)
		{
			this.lineno = line;
		}
		public static Node newNumber(double number)
		{
			return new Node.NumberNode(number);
		}
		public static Node newString(string str)
		{
			return new Node.StringNode(40, str);
		}
		public static Node newString(int Type, string str)
		{
			return new Node.StringNode(Type, str);
		}
		public bool hasChildren()
		{
			return this.first != null;
		}
		public Node getChildBefore(Node child)
		{
			Node result;
			if (child == this.first)
			{
				result = null;
			}
			else
			{
				Node node = this.first;
				while (node.next != child)
				{
					node = node.next;
					if (node == null)
					{
						throw new ApplicationException("node is not a child");
					}
				}
				result = node;
			}
			return result;
		}
		public void addChildToFront(Node child)
		{
			child.next = this.first;
			this.first = child;
			if (this.last == null)
			{
				this.last = child;
			}
		}
		public void addChildToBack(Node child)
		{
			child.next = null;
			if (this.last == null)
			{
				this.last = child;
				this.first = child;
			}
			else
			{
				this.last.next = child;
				this.last = child;
			}
		}
		public void addChildrenToFront(Node children)
		{
			Node lastSibling = children.LastSibling;
			lastSibling.next = this.first;
			this.first = children;
			if (this.last == null)
			{
				this.last = lastSibling;
			}
		}
		public void addChildrenToBack(Node children)
		{
			if (this.last != null)
			{
				this.last.next = children;
			}
			this.last = children.LastSibling;
			if (this.first == null)
			{
				this.first = children;
			}
		}
		public void addChildBefore(Node newChild, Node node)
		{
			if (newChild.next != null)
			{
				throw new ApplicationException("newChild had siblings in addChildBefore");
			}
			if (this.first == node)
			{
				newChild.next = this.first;
				this.first = newChild;
			}
			else
			{
				Node childBefore = this.getChildBefore(node);
				this.addChildAfter(newChild, childBefore);
			}
		}
		public void addChildAfter(Node newChild, Node node)
		{
			if (newChild.next != null)
			{
				throw new ApplicationException("newChild had siblings in addChildAfter");
			}
			newChild.next = node.next;
			node.next = newChild;
			if (this.last == node)
			{
				this.last = newChild;
			}
		}
		public void removeChild(Node child)
		{
			Node childBefore = this.getChildBefore(child);
			if (childBefore == null)
			{
				this.first = this.first.next;
			}
			else
			{
				childBefore.next = child.next;
			}
			if (child == this.last)
			{
				this.last = childBefore;
			}
			child.next = null;
		}
		public void replaceChild(Node child, Node newChild)
		{
			newChild.next = child.next;
			if (child == this.first)
			{
				this.first = newChild;
			}
			else
			{
				Node childBefore = this.getChildBefore(child);
				childBefore.next = newChild;
			}
			if (child == this.last)
			{
				this.last = newChild;
			}
			child.next = null;
		}
		public void replaceChildAfter(Node prevChild, Node newChild)
		{
			Node node = prevChild.next;
			newChild.next = node.next;
			prevChild.next = newChild;
			if (node == this.last)
			{
				this.last = newChild;
			}
			node.next = null;
		}
		private static string propToString(int propType)
		{
			string result;
			if (Token.printTrees)
			{
				switch (propType)
				{
				case 1:
					result = "function";
					return result;
				case 2:
					result = "local";
					return result;
				case 3:
					result = "local_block";
					return result;
				case 4:
					result = "regexp";
					return result;
				case 5:
					result = "casearray";
					return result;
				case 6:
					result = "targetblock";
					return result;
				case 7:
					result = "variable";
					return result;
				case 8:
					result = "isnumber";
					return result;
				case 9:
					result = "directcall";
					return result;
				case 10:
					result = "specialcall";
					return result;
				case 11:
					result = "skip_indexes";
					return result;
				case 12:
					result = "object_ids_prop";
					return result;
				case 13:
					result = "incrdecr_prop";
					return result;
				case 14:
					result = "catch_scope_prop";
					return result;
				case 15:
					result = "label_id_prop";
					return result;
				case 16:
					result = "member_Type_prop";
					return result;
				case 17:
					result = "name_prop";
					return result;
				default:
					Context.CodeBug();
					break;
				}
			}
			result = null;
			return result;
		}
		private Node.PropListItem lookupProperty(int propType)
		{
			Node.PropListItem propListItem = this.propListHead;
			while (propListItem != null && propType != propListItem.Type)
			{
				propListItem = propListItem.next;
			}
			return propListItem;
		}
		private Node.PropListItem ensureProperty(int propType)
		{
			Node.PropListItem propListItem = this.lookupProperty(propType);
			if (propListItem == null)
			{
				propListItem = new Node.PropListItem();
				propListItem.Type = propType;
				propListItem.next = this.propListHead;
				this.propListHead = propListItem;
			}
			return propListItem;
		}
		public void removeProp(int propType)
		{
			Node.PropListItem propListItem = this.propListHead;
			if (propListItem != null)
			{
				Node.PropListItem propListItem2 = null;
				while (propListItem.Type != propType)
				{
					propListItem2 = propListItem;
					propListItem = propListItem.next;
					if (propListItem == null)
					{
						return;
					}
				}
				if (propListItem2 == null)
				{
					this.propListHead = propListItem.next;
				}
				else
				{
					propListItem2.next = propListItem.next;
				}
			}
		}
		public object getProp(int propType)
		{
			Node.PropListItem propListItem = this.lookupProperty(propType);
			object result;
			if (propListItem == null)
			{
				result = null;
			}
			else
			{
				result = propListItem.objectValue;
			}
			return result;
		}
		public int getIntProp(int propType, int defaultValue)
		{
			Node.PropListItem propListItem = this.lookupProperty(propType);
			int result;
			if (propListItem == null)
			{
				result = defaultValue;
			}
			else
			{
				result = propListItem.intValue;
			}
			return result;
		}
		public int getExistingIntProp(int propType)
		{
			Node.PropListItem propListItem = this.lookupProperty(propType);
			if (propListItem == null)
			{
				Context.CodeBug();
			}
			return propListItem.intValue;
		}
		public void putProp(int propType, object prop)
		{
			if (prop == null)
			{
				this.removeProp(propType);
			}
			else
			{
				Node.PropListItem propListItem = this.ensureProperty(propType);
				propListItem.objectValue = prop;
			}
		}
		public void putIntProp(int propType, int prop)
		{
			Node.PropListItem propListItem = this.ensureProperty(propType);
			propListItem.intValue = prop;
		}
		public static Node newTarget()
		{
			return new Node(129);
		}
		public int labelId()
		{
			if (this.Type != 129)
			{
				Context.CodeBug();
			}
			return this.getIntProp(15, -1);
		}
		public void labelId(int labelId)
		{
			if (this.Type != 129)
			{
				Context.CodeBug();
			}
			this.putIntProp(15, labelId);
		}
		public override string ToString()
		{
			string result;
			if (Token.printTrees)
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.toString(new ObjToIntMap(), stringBuilder);
				result = stringBuilder.ToString();
			}
			else
			{
				result = Convert.ToString(this.Type);
			}
			return result;
		}
		private void toString(ObjToIntMap printIds, StringBuilder sb)
		{
			if (Token.printTrees)
			{
				sb.Append(Token.name(this.Type));
				if (this is Node.StringNode)
				{
					sb.Append(' ');
					sb.Append(this.String);
				}
				else
				{
					if (this is ScriptOrFnNode)
					{
						ScriptOrFnNode scriptOrFnNode = (ScriptOrFnNode)this;
						if (this is FunctionNode)
						{
							FunctionNode functionNode = (FunctionNode)this;
							sb.Append(' ');
							sb.Append(functionNode.FunctionName);
						}
						sb.Append(" [source name: ");
						sb.Append(scriptOrFnNode.SourceName);
						sb.Append("] [encoded source length: ");
						sb.Append(scriptOrFnNode.EncodedSourceEnd - scriptOrFnNode.EncodedSourceStart);
						sb.Append("] [base line: ");
						sb.Append(scriptOrFnNode.BaseLineno);
						sb.Append("] [end line: ");
						sb.Append(scriptOrFnNode.EndLineno);
						sb.Append(']');
					}
					else
					{
						if (this is Node.Jump)
						{
							Node.Jump jump = (Node.Jump)this;
							if (this.Type == 118 || this.Type == 119)
							{
								sb.Append(" [label: ");
								Node.appendPrintId(jump.JumpStatement, printIds, sb);
								sb.Append(']');
							}
							else
							{
								if (this.Type == 79)
								{
									Node target = jump.target;
									Node @finally = jump.Finally;
									if (target != null)
									{
										sb.Append(" [catch: ");
										Node.appendPrintId(target, printIds, sb);
										sb.Append(']');
									}
									if (@finally != null)
									{
										sb.Append(" [finally: ");
										Node.appendPrintId(@finally, printIds, sb);
										sb.Append(']');
									}
								}
								else
								{
									if (this.Type == 128 || this.Type == 130 || this.Type == 112)
									{
										sb.Append(" [break: ");
										Node.appendPrintId(jump.target, printIds, sb);
										sb.Append(']');
										if (this.Type == 130)
										{
											sb.Append(" [continue: ");
											Node.appendPrintId(jump.Continue, printIds, sb);
											sb.Append(']');
										}
									}
									else
									{
										sb.Append(" [target: ");
										Node.appendPrintId(jump.target, printIds, sb);
										sb.Append(']');
									}
								}
							}
						}
						else
						{
							if (this.Type == 39)
							{
								sb.Append(' ');
								sb.Append(this.Double);
							}
							else
							{
								if (this.Type == 129)
								{
									sb.Append(' ');
									Node.appendPrintId(this, printIds, sb);
								}
							}
						}
					}
				}
				if (this.lineno != -1)
				{
					sb.Append(' ');
					sb.Append(this.lineno);
				}
				for (Node.PropListItem propListItem = this.propListHead; propListItem != null; propListItem = propListItem.next)
				{
					int type = propListItem.Type;
					sb.Append(" [");
					sb.Append(Node.propToString(type));
					sb.Append(": ");
					int num = type;
					string value;
					if (num != 3)
					{
						switch (num)
						{
						case 6:
							value = "target block property";
							goto IL_4A9;
						case 8:
							switch (propListItem.intValue)
							{
							case 0:
								value = "both";
								break;
							case 1:
								value = "left";
								break;
							case 2:
								value = "right";
								break;
							default:
								throw Context.CodeBug();
							}
							goto IL_4A9;
						case 10:
							switch (propListItem.intValue)
							{
							case 1:
								value = "eval";
								break;
							case 2:
								value = "with";
								break;
							default:
								throw Context.CodeBug();
							}
							goto IL_4A9;
						}
						object objectValue = propListItem.objectValue;
						if (objectValue != null)
						{
							value = objectValue.ToString();
						}
						else
						{
							value = Convert.ToString(propListItem.intValue);
						}
					}
					else
					{
						value = "last local block";
					}
					IL_4A9:
					sb.Append(value);
					sb.Append(']');
				}
			}
		}
		public string toStringTree(ScriptOrFnNode treeTop)
		{
			string result;
			if (Token.printTrees)
			{
				StringBuilder stringBuilder = new StringBuilder();
				Node.toStringTreeHelper(treeTop, this, null, 0, stringBuilder);
				result = stringBuilder.ToString();
			}
			else
			{
				result = null;
			}
			return result;
		}
		private static void toStringTreeHelper(ScriptOrFnNode treeTop, Node n, ObjToIntMap printIds, int level, StringBuilder sb)
		{
			if (Token.printTrees)
			{
				if (printIds == null)
				{
					printIds = new ObjToIntMap();
					Node.generatePrintIds(treeTop, printIds);
				}
				for (int num = 0; num != level; num++)
				{
					sb.Append(" ");
				}
				n.toString(printIds, sb);
				sb.Append('\n');
				for (Node firstChild = n.FirstChild; firstChild != null; firstChild = firstChild.Next)
				{
					if (firstChild.Type == 107)
					{
						int existingIntProp = firstChild.getExistingIntProp(1);
						FunctionNode functionNode = treeTop.getFunctionNode(existingIntProp);
						Node.toStringTreeHelper(functionNode, functionNode, null, level + 1, sb);
					}
					else
					{
						Node.toStringTreeHelper(treeTop, firstChild, printIds, level + 1, sb);
					}
				}
			}
		}
		private static void generatePrintIds(Node n, ObjToIntMap map)
		{
			if (Token.printTrees)
			{
				map.put(n, map.size());
				for (Node firstChild = n.FirstChild; firstChild != null; firstChild = firstChild.Next)
				{
					Node.generatePrintIds(firstChild, map);
				}
			}
		}
		private static void appendPrintId(Node n, ObjToIntMap printIds, StringBuilder sb)
		{
			if (Token.printTrees)
			{
				if (n != null)
				{
					int num = printIds.Get(n, -1);
					sb.Append('#');
					if (num != -1)
					{
						sb.Append(num + 1);
					}
					else
					{
						sb.Append("<not_available>");
					}
				}
			}
		}
	}
}
