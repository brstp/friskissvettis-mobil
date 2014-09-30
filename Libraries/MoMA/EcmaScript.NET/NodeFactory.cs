using EcmaScript.NET.Collections;
using System;
namespace EcmaScript.NET
{
	internal sealed class NodeFactory
	{
		private const int LOOP_DO_WHILE = 0;
		private const int LOOP_WHILE = 1;
		private const int LOOP_FOR = 2;
		private const int ALWAYS_TRUE_BOOLEAN = 1;
		private const int ALWAYS_FALSE_BOOLEAN = -1;
		private Parser parser;
		internal NodeFactory(Parser parser)
		{
			this.parser = parser;
		}
		internal ScriptOrFnNode CreateScript()
		{
			return new ScriptOrFnNode(134);
		}
		internal void initScript(ScriptOrFnNode scriptNode, Node body)
		{
			Node firstChild = body.FirstChild;
			if (firstChild != null)
			{
				scriptNode.addChildrenToBack(firstChild);
			}
		}
		internal Node CreateLeaf(int nodeType)
		{
			return new Node(nodeType);
		}
		internal Node CreateLeaf(int nodeType, int nodeOp)
		{
			return new Node(nodeType, nodeOp);
		}
		internal Node CreateSwitch(Node expr, int lineno)
		{
			Node.Jump child = new Node.Jump(112, expr, lineno);
			return new Node(127, child);
		}
		internal void addSwitchCase(Node switchBlock, Node caseExpression, Node statements)
		{
			if (switchBlock.Type != 127)
			{
				throw Context.CodeBug();
			}
			Node.Jump jump = (Node.Jump)switchBlock.FirstChild;
			if (jump.Type != 112)
			{
				throw Context.CodeBug();
			}
			Node node = Node.newTarget();
			if (caseExpression != null)
			{
				jump.addChildToBack(new Node.Jump(113, caseExpression)
				{
					target = node
				});
			}
			else
			{
				jump.Default = node;
			}
			switchBlock.addChildToBack(node);
			switchBlock.addChildToBack(statements);
		}
		internal void closeSwitch(Node switchBlock)
		{
			if (switchBlock.Type != 127)
			{
				throw Context.CodeBug();
			}
			Node.Jump jump = (Node.Jump)switchBlock.FirstChild;
			if (jump.Type != 112)
			{
				throw Context.CodeBug();
			}
			Node node = Node.newTarget();
			jump.target = node;
			Node node2 = jump.Default;
			if (node2 == null)
			{
				node2 = node;
			}
			switchBlock.addChildAfter(this.makeJump(5, node2), jump);
			switchBlock.addChildToBack(node);
		}
		internal Node CreateVariables(int lineno)
		{
			return new Node(120, lineno);
		}
		internal Node CreateExprStatement(Node expr, int lineno)
		{
			int nodeType;
			if (this.parser.insideFunction())
			{
				nodeType = 131;
			}
			else
			{
				nodeType = 132;
			}
			return new Node(nodeType, expr, lineno);
		}
		internal Node CreateExprStatementNoReturn(Node expr, int lineno)
		{
			return new Node(131, expr, lineno);
		}
		internal Node CreateDefaultNamespace(Node expr, int lineno)
		{
			this.setRequiresActivation();
			Node expr2 = this.CreateUnary(70, expr);
			return this.CreateExprStatement(expr2, lineno);
		}
		internal Node CreateName(string name)
		{
			this.checkActivationName(name, 38);
			return Node.newString(38, name);
		}
		internal Node CreateString(string str)
		{
			return Node.newString(str);
		}
		internal Node CreateNumber(double number)
		{
			return Node.newNumber(number);
		}
		internal Node CreateCatch(string varName, Node catchCond, Node stmts, int lineno)
		{
			if (catchCond == null)
			{
				catchCond = new Node(126);
			}
			return new Node(122, this.CreateName(varName), catchCond, stmts, lineno);
		}
		internal Node CreateThrow(Node expr, int lineno)
		{
			return new Node(49, expr, lineno);
		}
		internal Node CreateReturn(Node expr, int lineno)
		{
			return (expr == null) ? new Node(4, lineno) : new Node(4, expr, lineno);
		}
		internal Node CreateDebugger(int lineno)
		{
			return new Node(156, lineno);
		}
		internal Node CreateLabel(int lineno)
		{
			return new Node.Jump(128, lineno);
		}
		internal Node getLabelLoop(Node label)
		{
			return ((Node.Jump)label).Loop;
		}
		internal Node CreateLabeledStatement(Node labelArg, Node statement)
		{
			Node.Jump jump = (Node.Jump)labelArg;
			Node node = Node.newTarget();
			Node result = new Node(127, jump, statement, node);
			jump.target = node;
			return result;
		}
		internal Node CreateBreak(Node breakStatement, int lineno)
		{
			Node.Jump jump = new Node.Jump(118, lineno);
			int type = breakStatement.Type;
			Node.Jump jumpStatement;
			if (type == 130 || type == 128)
			{
				jumpStatement = (Node.Jump)breakStatement;
			}
			else
			{
				if (type != 127 || breakStatement.FirstChild.Type != 112)
				{
					throw Context.CodeBug();
				}
				jumpStatement = (Node.Jump)breakStatement.FirstChild;
			}
			jump.JumpStatement = jumpStatement;
			return jump;
		}
		internal Node CreateContinue(Node loop, int lineno)
		{
			if (loop.Type != 130)
			{
				Context.CodeBug();
			}
			return new Node.Jump(119, lineno)
			{
				JumpStatement = (Node.Jump)loop
			};
		}
		internal Node CreateBlock(int lineno)
		{
			return new Node(127, lineno);
		}
		internal FunctionNode CreateFunction(string name)
		{
			return new FunctionNode(name);
		}
		internal Node initFunction(FunctionNode fnNode, int functionIndex, Node statements, int functionType)
		{
			fnNode.itsFunctionType = functionType;
			fnNode.addChildToBack(statements);
			int functionCount = fnNode.FunctionCount;
			if (functionCount != 0)
			{
				fnNode.itsNeedsActivation = true;
				for (int num = 0; num != functionCount; num++)
				{
					FunctionNode functionNode = fnNode.getFunctionNode(num);
					if (functionNode.FunctionType == 3)
					{
						string functionName = functionNode.FunctionName;
						if (functionName != null && functionName.Length != 0)
						{
							fnNode.removeParamOrVar(functionName);
						}
					}
				}
			}
			if (functionType == 2)
			{
				string functionName = fnNode.FunctionName;
				if (functionName != null && functionName.Length != 0 && !fnNode.hasParamOrVar(functionName))
				{
					fnNode.addVar(functionName);
					Node children = new Node(131, new Node(8, Node.newString(48, functionName), new Node(61)));
					statements.addChildrenToFront(children);
				}
			}
			Node lastChild = statements.LastChild;
			if (lastChild == null || lastChild.Type != 4)
			{
				statements.addChildToBack(new Node(4));
			}
			Node node = Node.newString(107, fnNode.FunctionName);
			node.putIntProp(1, functionIndex);
			return node;
		}
		internal void addChildToBack(Node parent, Node child)
		{
			parent.addChildToBack(child);
		}
		internal Node CreateLoopNode(Node loopLabel, int lineno)
		{
			Node.Jump jump = new Node.Jump(130, lineno);
			if (loopLabel != null)
			{
				((Node.Jump)loopLabel).Loop = jump;
			}
			return jump;
		}
		internal Node CreateWhile(Node loop, Node cond, Node body)
		{
			return this.CreateLoop((Node.Jump)loop, 1, body, cond, null, null);
		}
		internal Node CreateDoWhile(Node loop, Node body, Node cond)
		{
			return this.CreateLoop((Node.Jump)loop, 0, body, cond, null, null);
		}
		internal Node CreateFor(Node loop, Node init, Node test, Node incr, Node body)
		{
			return this.CreateLoop((Node.Jump)loop, 2, body, test, init, incr);
		}
		private Node CreateLoop(Node.Jump loop, int loopType, Node body, Node cond, Node init, Node incr)
		{
			Node node = Node.newTarget();
			Node node2 = Node.newTarget();
			if (loopType == 2 && cond.Type == 126)
			{
				cond = new Node(44);
			}
			Node.Jump jump = new Node.Jump(6, cond);
			jump.target = node;
			Node node3 = Node.newTarget();
			loop.addChildToBack(node);
			loop.addChildrenToBack(body);
			if (loopType == 1 || loopType == 2)
			{
				loop.addChildrenToBack(new Node(126, loop.Lineno));
			}
			loop.addChildToBack(node2);
			loop.addChildToBack(jump);
			loop.addChildToBack(node3);
			loop.target = node3;
			Node @continue = node2;
			if (loopType == 1 || loopType == 2)
			{
				loop.addChildToFront(this.makeJump(5, node2));
				if (loopType == 2)
				{
					if (init.Type != 126)
					{
						if (init.Type != 120)
						{
							init = new Node(131, init);
						}
						loop.addChildToFront(init);
					}
					Node node4 = Node.newTarget();
					loop.addChildAfter(node4, body);
					if (incr.Type != 126)
					{
						incr = new Node(131, incr);
						loop.addChildAfter(incr, node4);
					}
					@continue = node4;
				}
			}
			loop.Continue = @continue;
			return loop;
		}
		internal Node CreateForIn(Node loop, Node lhs, Node obj, Node body, bool isForEach)
		{
			int type = lhs.Type;
			Node node;
			Node result;
			if (type == 120)
			{
				Node lastChild = lhs.LastChild;
				if (lhs.FirstChild != lastChild)
				{
					this.parser.ReportError("msg.mult.index");
				}
				node = Node.newString(38, lastChild.String);
			}
			else
			{
				node = this.makeReference(lhs);
				if (node == null)
				{
					this.parser.ReportError("msg.bad.for.in.lhs");
					result = obj;
					return result;
				}
			}
			Node node2 = new Node(139);
			int nodeType = isForEach ? 58 : 57;
			Node node3 = new Node(nodeType, obj);
			node3.putProp(3, node2);
			Node node4 = new Node(59);
			node4.putProp(3, node2);
			Node node5 = new Node(60);
			node5.putProp(3, node2);
			Node node6 = new Node(127);
			Node child = this.simpleAssignment(node, node5);
			node6.addChildToBack(new Node(131, child));
			node6.addChildToBack(body);
			loop = this.CreateWhile(loop, node4, node6);
			loop.addChildToFront(node3);
			if (type == 120)
			{
				loop.addChildToFront(lhs);
			}
			node2.addChildToBack(loop);
			result = node2;
			return result;
		}
		internal Node CreateTryCatchFinally(Node tryBlock, Node catchBlocks, Node finallyBlock, int lineno)
		{
			bool flag = finallyBlock != null && (finallyBlock.Type != 127 || finallyBlock.hasChildren());
			Node result;
			if (tryBlock.Type == 127 && !tryBlock.hasChildren() && !flag)
			{
				result = tryBlock;
			}
			else
			{
				bool flag2 = catchBlocks.hasChildren();
				if (!flag && !flag2)
				{
					result = tryBlock;
				}
				else
				{
					Node node = new Node(139);
					Node.Jump jump = new Node.Jump(79, tryBlock, lineno);
					jump.putProp(3, node);
					if (flag2)
					{
						Node node2 = Node.newTarget();
						jump.addChildToBack(this.makeJump(5, node2));
						Node node3 = Node.newTarget();
						jump.target = node3;
						jump.addChildToBack(node3);
						Node node4 = new Node(139);
						Node node5 = catchBlocks.FirstChild;
						bool flag3 = false;
						int num = 0;
						while (node5 != null)
						{
							int lineno2 = node5.Lineno;
							Node firstChild = node5.FirstChild;
							Node next = firstChild.Next;
							Node next2 = next.Next;
							node5.removeChild(firstChild);
							node5.removeChild(next);
							node5.removeChild(next2);
							next2.addChildToBack(new Node(3));
							next2.addChildToBack(this.makeJump(5, node2));
							Node body;
							if (next.Type == 126)
							{
								body = next2;
								flag3 = true;
							}
							else
							{
								body = this.CreateIf(next, next2, null, lineno2);
							}
							Node node6 = new Node(56, firstChild, this.CreateUseLocal(node));
							node6.putProp(3, node4);
							node6.putIntProp(14, num);
							node4.addChildToBack(node6);
							node4.addChildToBack(this.CreateWith(this.CreateUseLocal(node4), body, lineno2));
							node5 = node5.Next;
							num++;
						}
						jump.addChildToBack(node4);
						if (!flag3)
						{
							Node node7 = new Node(50);
							node7.putProp(3, node);
							jump.addChildToBack(node7);
						}
						jump.addChildToBack(node2);
					}
					if (flag)
					{
						Node node8 = Node.newTarget();
						jump.Finally = node8;
						jump.addChildToBack(this.makeJump(133, node8));
						Node node9 = Node.newTarget();
						jump.addChildToBack(this.makeJump(5, node9));
						jump.addChildToBack(node8);
						Node node10 = new Node(123, finallyBlock);
						node10.putProp(3, node);
						jump.addChildToBack(node10);
						jump.addChildToBack(node9);
					}
					node.addChildToBack(jump);
					result = node;
				}
			}
			return result;
		}
		internal Node CreateWith(Node obj, Node body, int lineno)
		{
			this.setRequiresActivation();
			Node node = new Node(127, lineno);
			node.addChildToBack(new Node(2, obj));
			Node children = new Node(121, body, lineno);
			node.addChildrenToBack(children);
			node.addChildToBack(new Node(3));
			return node;
		}
		public Node CreateDotQuery(Node obj, Node body, int lineno)
		{
			this.setRequiresActivation();
			return new Node(144, obj, body, lineno);
		}
		internal Node CreateArrayLiteral(ObjArray elems, int skipCount)
		{
			int num = elems.size();
			int[] array = null;
			if (skipCount != 0)
			{
				array = new int[skipCount];
			}
			Node node = new Node(63);
			int num2 = 0;
			int num3 = 0;
			while (num2 != num)
			{
				Node node2 = (Node)elems.Get(num2);
				if (node2 != null)
				{
					node.addChildToBack(node2);
				}
				else
				{
					array[num3] = num2;
					num3++;
				}
				num2++;
			}
			if (skipCount != 0)
			{
				node.putProp(11, array);
			}
			return node;
		}
		internal Node CreateObjectLiteral(ObjArray elems)
		{
			int num = elems.size() / 2;
			Node node = new Node(64);
			object[] array;
			if (num == 0)
			{
				array = ScriptRuntime.EmptyArgs;
			}
			else
			{
				array = new object[num];
				for (int num2 = 0; num2 != num; num2++)
				{
					array[num2] = elems.Get(2 * num2);
					Node child = (Node)elems.Get(2 * num2 + 1);
					node.addChildToBack(child);
				}
			}
			node.putProp(12, array);
			return node;
		}
		internal Node CreateRegExp(int regexpIndex)
		{
			Node node = new Node(47);
			node.putIntProp(4, regexpIndex);
			return node;
		}
		internal Node CreateIf(Node cond, Node ifTrue, Node ifFalse, int lineno)
		{
			int num = NodeFactory.isAlwaysDefinedBoolean(cond);
			Node result;
			if (num == 1)
			{
				result = ifTrue;
			}
			else
			{
				if (num == -1)
				{
					if (ifFalse != null)
					{
						result = ifFalse;
					}
					else
					{
						result = new Node(127, lineno);
					}
				}
				else
				{
					Node node = new Node(127, lineno);
					Node node2 = Node.newTarget();
					node.addChildToBack(new Node.Jump(7, cond)
					{
						target = node2
					});
					node.addChildrenToBack(ifTrue);
					if (ifFalse != null)
					{
						Node node3 = Node.newTarget();
						node.addChildToBack(this.makeJump(5, node3));
						node.addChildToBack(node2);
						node.addChildrenToBack(ifFalse);
						node.addChildToBack(node3);
					}
					else
					{
						node.addChildToBack(node2);
					}
					result = node;
				}
			}
			return result;
		}
		internal Node CreateCondExpr(Node cond, Node ifTrue, Node ifFalse)
		{
			int num = NodeFactory.isAlwaysDefinedBoolean(cond);
			Node result;
			if (num == 1)
			{
				result = ifTrue;
			}
			else
			{
				if (num == -1)
				{
					result = ifFalse;
				}
				else
				{
					result = new Node(100, cond, ifTrue, ifFalse);
				}
			}
			return result;
		}
		internal Node CreateUnary(int nodeType, Node child)
		{
			int type = child.Type;
			Node result;
			switch (nodeType)
			{
			case 26:
			{
				int num = NodeFactory.isAlwaysDefinedBoolean(child);
				if (num != 0)
				{
					int num2;
					if (num == 1)
					{
						num2 = 43;
					}
					else
					{
						num2 = 44;
					}
					if (type == 44 || type == 43)
					{
						child.Type = num2;
						result = child;
						return result;
					}
					result = new Node(num2);
					return result;
				}
				break;
			}
			case 27:
				if (type == 39)
				{
					int num3 = ScriptConvert.ToInt32(child.Double);
					child.Double = (double)(~(double)num3);
					result = child;
					return result;
				}
				break;
			case 29:
				if (type == 39)
				{
					child.Double = -child.Double;
					result = child;
					return result;
				}
				break;
			case 31:
			{
				Node node2;
				if (type == 38)
				{
					child.Type = 48;
					Node node = Node.newString(child.String);
					node2 = new Node(nodeType, child, node);
				}
				else
				{
					if (type == 33 || type == 35)
					{
						Node firstChild = child.FirstChild;
						Node node = child.LastChild;
						child.removeChild(firstChild);
						child.removeChild(node);
						node2 = new Node(nodeType, firstChild, node);
					}
					else
					{
						if (type == 65)
						{
							Node firstChild2 = child.FirstChild;
							child.removeChild(firstChild2);
							node2 = new Node(67, firstChild2);
						}
						else
						{
							node2 = new Node(44);
						}
					}
				}
				result = node2;
				return result;
			}
			case 32:
				if (type == 38)
				{
					child.Type = 135;
					result = child;
					return result;
				}
				break;
			}
			result = new Node(nodeType, child);
			return result;
		}
		internal Node CreateCallOrNew(int nodeType, Node child)
		{
			int num = 0;
			if (child.Type == 38)
			{
				string @string = child.String;
				if (@string.Equals("eval"))
				{
					num = 1;
				}
				else
				{
					if (@string.Equals("With"))
					{
						num = 2;
					}
				}
			}
			else
			{
				if (child.Type == 33)
				{
					string @string = child.LastChild.String;
					if (@string.Equals("eval"))
					{
						num = 1;
					}
				}
			}
			Node node = new Node(nodeType, child);
			if (num != 0)
			{
				this.setRequiresActivation();
				node.putIntProp(10, num);
			}
			return node;
		}
		internal Node CreateIncDec(int nodeType, bool post, Node child)
		{
			child = this.makeReference(child);
			Node result;
			if (child != null)
			{
				int type = child.Type;
				int num = type;
				switch (num)
				{
				case 33:
				case 35:
					break;
				case 34:
					goto IL_D3;
				default:
					if (num != 38 && num != 65)
					{
						goto IL_D3;
					}
					break;
				}
				Node node = new Node(nodeType, child);
				int num2 = 0;
				if (nodeType == 105)
				{
					num2 |= 1;
				}
				if (post)
				{
					num2 |= 2;
				}
				node.putIntProp(13, num2);
				result = node;
				return result;
				IL_D3:
				throw Context.CodeBug();
			}
			string messageId;
			if (nodeType == 105)
			{
				messageId = "msg.bad.decr";
			}
			else
			{
				messageId = "msg.bad.incr";
			}
			this.parser.ReportError(messageId);
			result = null;
			return result;
		}
		internal Node CreatePropertyGet(Node target, string ns, string name, int memberTypeFlags)
		{
			Node result;
			if (ns == null && memberTypeFlags == 0)
			{
				if (target == null)
				{
					result = this.CreateName(name);
				}
				else
				{
					this.checkActivationName(name, 33);
					if (ScriptRuntime.isSpecialProperty(name))
					{
						Node node = new Node(69, target);
						node.putProp(17, name);
						result = new Node(65, node);
					}
					else
					{
						result = new Node(33, target, this.CreateString(name));
					}
				}
			}
			else
			{
				Node elem = this.CreateString(name);
				memberTypeFlags |= 1;
				result = this.CreateMemberRefGet(target, ns, elem, memberTypeFlags);
			}
			return result;
		}
		internal Node CreateElementGet(Node target, string ns, Node elem, int memberTypeFlags)
		{
			Node result;
			if (ns == null && memberTypeFlags == 0)
			{
				if (target == null)
				{
					throw Context.CodeBug();
				}
				result = new Node(35, target, elem);
			}
			else
			{
				result = this.CreateMemberRefGet(target, ns, elem, memberTypeFlags);
			}
			return result;
		}
		private Node CreateMemberRefGet(Node target, string ns, Node elem, int memberTypeFlags)
		{
			Node node = null;
			if (ns != null)
			{
				if (ns.Equals("*"))
				{
					node = new Node(41);
				}
				else
				{
					node = this.CreateName(ns);
				}
			}
			Node node2;
			if (target == null)
			{
				if (ns == null)
				{
					node2 = new Node(75, elem);
				}
				else
				{
					node2 = new Node(76, node, elem);
				}
			}
			else
			{
				if (ns == null)
				{
					node2 = new Node(73, target, elem);
				}
				else
				{
					node2 = new Node(74, target, node, elem);
				}
			}
			if (memberTypeFlags != 0)
			{
				node2.putIntProp(16, memberTypeFlags);
			}
			return new Node(65, node2);
		}
		internal Node CreateBinary(int nodeType, Node left, Node right)
		{
			Node result;
			switch (nodeType)
			{
			case 21:
				if (left.Type == 40)
				{
					string str;
					if (right.Type == 40)
					{
						str = right.String;
					}
					else
					{
						if (right.Type != 39)
						{
							break;
						}
						str = ScriptConvert.ToString(right.Double, 10);
					}
					string str2 = left.String;
					left.String = str2 + str;
					result = left;
					return result;
				}
				if (left.Type == 39)
				{
					if (right.Type == 39)
					{
						left.Double += right.Double;
						result = left;
						return result;
					}
					if (right.Type == 40)
					{
						string str2 = ScriptConvert.ToString(left.Double, 10);
						string str = right.String;
						right.String = str2 + str;
						result = right;
						return result;
					}
				}
				break;
			case 22:
				if (left.Type == 39)
				{
					double @double = left.Double;
					if (right.Type == 39)
					{
						left.Double = @double - right.Double;
						result = left;
						return result;
					}
					if (@double == 0.0)
					{
						result = new Node(29, right);
						return result;
					}
				}
				else
				{
					if (right.Type == 39)
					{
						if (right.Double == 0.0)
						{
							result = new Node(28, left);
							return result;
						}
					}
				}
				break;
			case 23:
				if (left.Type == 39)
				{
					double @double = left.Double;
					if (right.Type == 39)
					{
						left.Double = @double * right.Double;
						result = left;
						return result;
					}
					if (@double == 1.0)
					{
						result = new Node(28, right);
						return result;
					}
				}
				else
				{
					if (right.Type == 39)
					{
						if (right.Double == 1.0)
						{
							result = new Node(28, left);
							return result;
						}
					}
				}
				break;
			case 24:
				if (right.Type == 39)
				{
					double double2 = right.Double;
					if (left.Type == 39)
					{
						left.Double /= double2;
						result = left;
						return result;
					}
					if (double2 == 1.0)
					{
						result = new Node(28, left);
						return result;
					}
				}
				break;
			default:
				switch (nodeType)
				{
				case 102:
				{
					int num = NodeFactory.isAlwaysDefinedBoolean(left);
					if (num == 1)
					{
						result = new Node(44);
						return result;
					}
					if (num == -1)
					{
						result = right;
						return result;
					}
					int num2 = NodeFactory.isAlwaysDefinedBoolean(right);
					if (num2 == 1)
					{
						if (!NodeFactory.hasSideEffects(left))
						{
							result = new Node(44);
							return result;
						}
					}
					else
					{
						if (num2 == -1)
						{
							result = left;
							return result;
						}
					}
					break;
				}
				case 103:
				{
					int num = NodeFactory.isAlwaysDefinedBoolean(left);
					if (num == -1)
					{
						result = new Node(43);
						return result;
					}
					if (num == 1)
					{
						result = right;
						return result;
					}
					int num2 = NodeFactory.isAlwaysDefinedBoolean(right);
					if (num2 == -1)
					{
						if (!NodeFactory.hasSideEffects(left))
						{
							result = new Node(43);
							return result;
						}
					}
					else
					{
						if (num2 == 1)
						{
							result = left;
							return result;
						}
					}
					break;
				}
				}
				break;
			}
			result = new Node(nodeType, left, right);
			return result;
		}
		private Node simpleAssignment(Node left, Node right)
		{
			int type = left.Type;
			int num = type;
			switch (num)
			{
			case 33:
			case 35:
			{
				Node firstChild = left.FirstChild;
				Node lastChild = left.LastChild;
				int nodeType;
				if (type == 33)
				{
					nodeType = 34;
				}
				else
				{
					nodeType = 36;
				}
				Node result = new Node(nodeType, firstChild, lastChild, right);
				return result;
			}
			case 34:
				break;
			default:
				if (num == 38)
				{
					left.Type = 48;
					Node result = new Node(8, left, right);
					return result;
				}
				if (num == 65)
				{
					Node firstChild2 = left.FirstChild;
					this.checkMutableReference(firstChild2);
					Node result = new Node(66, firstChild2, right);
					return result;
				}
				break;
			}
			throw Context.CodeBug();
		}
		private void checkMutableReference(Node n)
		{
			int intProp = n.getIntProp(16, 0);
			if ((intProp & 4) != 0)
			{
				this.parser.ReportError("msg.bad.assign.left");
			}
		}
		internal Node CreateAssignment(int assignType, Node left, Node right)
		{
			left = this.makeReference(left);
			Node result;
			if (left != null)
			{
				int nodeType;
				switch (assignType)
				{
				case 88:
					result = this.simpleAssignment(left, right);
					return result;
				case 89:
					nodeType = 9;
					break;
				case 90:
					nodeType = 10;
					break;
				case 91:
					nodeType = 11;
					break;
				case 92:
					nodeType = 18;
					break;
				case 93:
					nodeType = 19;
					break;
				case 94:
					nodeType = 20;
					break;
				case 95:
					nodeType = 21;
					break;
				case 96:
					nodeType = 22;
					break;
				case 97:
					nodeType = 23;
					break;
				case 98:
					nodeType = 24;
					break;
				case 99:
					nodeType = 25;
					break;
				default:
					throw Context.CodeBug();
				}
				int type = left.Type;
				int num = type;
				switch (num)
				{
				case 33:
				case 35:
				{
					Node firstChild = left.FirstChild;
					Node lastChild = left.LastChild;
					int nodeType2 = (type == 33) ? 137 : 138;
					Node left2 = new Node(136);
					Node right2 = new Node(nodeType, left2, right);
					result = new Node(nodeType2, firstChild, lastChild, right2);
					return result;
				}
				case 34:
					break;
				default:
					if (num == 38)
					{
						string @string = left.String;
						Node left2 = Node.newString(38, @string);
						Node right2 = new Node(nodeType, left2, right);
						Node left3 = Node.newString(48, @string);
						result = new Node(8, left3, right2);
						return result;
					}
					if (num == 65)
					{
						Node firstChild2 = left.FirstChild;
						this.checkMutableReference(firstChild2);
						Node left2 = new Node(136);
						Node right2 = new Node(nodeType, left2, right);
						result = new Node(140, firstChild2, right2);
						return result;
					}
					break;
				}
				throw Context.CodeBug();
			}
			this.parser.ReportError("msg.bad.assign.left");
			result = right;
			return result;
		}
		internal Node CreateUseLocal(Node localBlock)
		{
			if (139 != localBlock.Type)
			{
				throw Context.CodeBug();
			}
			Node node = new Node(53);
			node.putProp(3, localBlock);
			return node;
		}
		private Node.Jump makeJump(int type, Node target)
		{
			return new Node.Jump(type)
			{
				target = target
			};
		}
		private Node makeReference(Node node)
		{
			int type = node.Type;
			int num = type;
			Node result;
			switch (num)
			{
			case 33:
			case 35:
			case 38:
				break;
			case 34:
			case 36:
				goto IL_55;
			case 37:
				node.Type = 68;
				result = new Node(65, node);
				return result;
			default:
				if (num != 65)
				{
					goto IL_55;
				}
				break;
			}
			result = node;
			return result;
			IL_55:
			result = null;
			return result;
		}
		private static int isAlwaysDefinedBoolean(Node node)
		{
			int result;
			switch (node.Type)
			{
			case 39:
			{
				double @double = node.Double;
				if (!double.IsNaN(@double) && @double != 0.0)
				{
					result = 1;
					return result;
				}
				result = -1;
				return result;
			}
			case 41:
			case 43:
				result = -1;
				return result;
			case 44:
				result = 1;
				return result;
			}
			result = 0;
			return result;
		}
		private static bool hasSideEffects(Node exprTree)
		{
			int type = exprTree.Type;
			if (type <= 30)
			{
				if (type != 8 && type != 30)
				{
					goto IL_5A;
				}
			}
			else
			{
				switch (type)
				{
				case 34:
				case 36:
				case 37:
					break;
				case 35:
					goto IL_5A;
				default:
					switch (type)
					{
					case 104:
					case 105:
						break;
					default:
						goto IL_5A;
					}
					break;
				}
			}
			bool result = true;
			return result;
			IL_5A:
			for (Node node = exprTree.FirstChild; node != null; node = node.Next)
			{
				if (NodeFactory.hasSideEffects(node))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		private void checkActivationName(string name, int token)
		{
			if (this.parser.insideFunction())
			{
				bool flag = false;
				if ("arguments".Equals(name) || (this.parser.compilerEnv.activationNames != null && this.parser.compilerEnv.activationNames.ContainsKey(name)))
				{
					flag = true;
				}
				else
				{
					if ("length".Equals(name))
					{
						if (token == 33 && this.parser.compilerEnv.LanguageVersion == Context.Versions.JS1_2)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					this.setRequiresActivation();
				}
			}
		}
		private void setRequiresActivation()
		{
			if (this.parser.insideFunction())
			{
				((FunctionNode)this.parser.currentScriptOrFn).itsNeedsActivation = true;
			}
		}
	}
}
