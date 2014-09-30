using EcmaScript.NET.Collections;
using EcmaScript.NET.Helpers;
using System;
using System.Runtime.InteropServices;
namespace EcmaScript.NET
{
	[ComVisible(true)]
	public class NodeTransformer
	{
		private ObjArray loops;
		private ObjArray loopEnds;
		private bool hasFinally;
		public void transform(ScriptOrFnNode tree)
		{
			this.transformCompilationUnit(tree);
			for (int num = 0; num != tree.FunctionCount; num++)
			{
				FunctionNode functionNode = tree.getFunctionNode(num);
				this.transform(functionNode);
			}
		}
		private void transformCompilationUnit(ScriptOrFnNode tree)
		{
			this.loops = new ObjArray();
			this.loopEnds = new ObjArray();
			this.hasFinally = false;
			try
			{
				this.transformCompilationUnit_r(tree, tree);
			}
			catch (StackOverflowVerifierException)
			{
				throw Context.ReportRuntimeError(ScriptRuntime.GetMessage("mag.too.deep.parser.recursion", new object[0]));
			}
		}
		private void transformCompilationUnit_r(ScriptOrFnNode tree, Node parent)
		{
			Node node = null;
			using (new StackOverflowVerifier(1024))
			{
				while (true)
				{
					Node previous = null;
					if (node == null)
					{
						node = parent.FirstChild;
					}
					else
					{
						previous = node;
						node = node.Next;
					}
					if (node == null)
					{
						break;
					}
					int type = node.Type;
					int num = type;
					if (num <= 31)
					{
						switch (num)
						{
						case 3:
							goto IL_1BE;
						case 4:
						{
							if (!this.hasFinally)
							{
								goto IL_6AC;
							}
							Node node2 = null;
							for (int i = this.loops.size() - 1; i >= 0; i--)
							{
								Node node3 = (Node)this.loops.Get(i);
								int type2 = node3.Type;
								if (type2 == 79 || type2 == 121)
								{
									Node child;
									if (type2 == 79)
									{
										Node.Jump jump = new Node.Jump(133);
										Node @finally = ((Node.Jump)node3).Finally;
										jump.target = @finally;
										child = jump;
									}
									else
									{
										child = new Node(3);
									}
									if (node2 == null)
									{
										node2 = new Node(127, node.Lineno);
									}
									node2.addChildToBack(child);
								}
							}
							if (node2 == null)
							{
								goto IL_6AC;
							}
							Node node4 = node;
							Node firstChild = node4.FirstChild;
							node = NodeTransformer.replaceCurrent(parent, previous, node, node2);
							if (firstChild == null)
							{
								node2.addChildToBack(node4);
							}
							else
							{
								Node node5 = new Node(132, firstChild);
								node2.addChildToFront(node5);
								node4 = new Node(62);
								node2.addChildToBack(node4);
								this.transformCompilationUnit_r(tree, node5);
							}
							break;
						}
						default:
							if (num == 8)
							{
								goto IL_590;
							}
							switch (num)
							{
							case 30:
								this.visitNew(node, tree);
								goto IL_6AC;
							case 31:
								goto IL_590;
							default:
								goto IL_6AC;
							}
							break;
						}
					}
					else
					{
						if (num > 79)
						{
							if (num != 112)
							{
								switch (num)
								{
								case 118:
								case 119:
								{
									Node.Jump jump2 = (Node.Jump)node;
									Node.Jump jumpStatement = jump2.JumpStatement;
									if (jumpStatement == null)
									{
										Context.CodeBug();
									}
									int i = this.loops.size();
									while (i != 0)
									{
										i--;
										Node node3 = (Node)this.loops.Get(i);
										if (node3 == jumpStatement)
										{
											if (type == 118)
											{
												jump2.target = jumpStatement.target;
											}
											else
											{
												jump2.target = jumpStatement.Continue;
											}
											jump2.Type = 5;
											goto IL_6AC;
										}
										int type2 = node3.Type;
										if (type2 == 121)
										{
											Node node6 = new Node(3);
											previous = NodeTransformer.addBeforeCurrent(parent, previous, node, node6);
										}
										else
										{
											if (type2 == 79)
											{
												Node.Jump jump3 = (Node.Jump)node3;
												previous = NodeTransformer.addBeforeCurrent(parent, previous, node, new Node.Jump(133)
												{
													target = jump3.Finally
												});
											}
										}
									}
									goto Block_27;
								}
								case 120:
								{
									Node node7 = new Node(127);
									Node node8 = node.FirstChild;
									while (node8 != null)
									{
										Node node3 = node8;
										if (node3.Type != 38)
										{
											Context.CodeBug();
										}
										node8 = node8.Next;
										if (node3.hasChildren())
										{
											Node firstChild2 = node3.FirstChild;
											node3.removeChild(firstChild2);
											node3.Type = 48;
											node3 = new Node(8, node3, firstChild2);
											Node child2 = new Node(131, node3, node.Lineno);
											node7.addChildToBack(child2);
										}
									}
									node = NodeTransformer.replaceCurrent(parent, previous, node, node7);
									goto IL_6AC;
								}
								case 121:
								{
									this.loops.push(node);
									Node node6 = node.Next;
									if (node6.Type != 3)
									{
										Context.CodeBug();
									}
									this.loopEnds.push(node6);
									goto IL_6AC;
								}
								case 122:
								case 123:
								case 124:
								case 125:
								case 126:
								case 127:
									goto IL_6AC;
								case 128:
								case 130:
									break;
								case 129:
									goto IL_1BE;
								default:
									goto IL_6AC;
								}
							}
							this.loops.push(node);
							this.loopEnds.push(((Node.Jump)node).target);
							goto IL_6AC;
						}
						switch (num)
						{
						case 37:
							this.visitCall(node, tree);
							goto IL_6AC;
						case 38:
							goto IL_590;
						default:
						{
							if (num != 79)
							{
								goto IL_6AC;
							}
							Node.Jump jump2 = (Node.Jump)node;
							Node finally2 = jump2.Finally;
							if (finally2 != null)
							{
								this.hasFinally = true;
								this.loops.push(node);
								this.loopEnds.push(finally2);
							}
							goto IL_6AC;
						}
						}
					}
					continue;
					IL_6AC:
					this.transformCompilationUnit_r(tree, node);
					continue;
					IL_1BE:
					if (!this.loopEnds.Empty && this.loopEnds.peek() == node)
					{
						this.loopEnds.pop();
						this.loops.pop();
					}
					goto IL_6AC;
					IL_590:
					if (tree.Type != 107 || ((FunctionNode)tree).RequiresActivation)
					{
						goto IL_6AC;
					}
					Node node9;
					if (type == 38)
					{
						node9 = node;
					}
					else
					{
						node9 = node.FirstChild;
						if (node9.Type != 48)
						{
							if (type == 31)
							{
								goto IL_6AC;
							}
							goto IL_60B;
						}
					}
					string @string = node9.String;
					if (tree.hasParamOrVar(@string))
					{
						if (type == 38)
						{
							node.Type = 54;
						}
						else
						{
							if (type == 8)
							{
								node.Type = 55;
								node9.Type = 40;
							}
							else
							{
								if (type != 31)
								{
									goto IL_69F;
								}
								Node node3 = new Node(43);
								node = NodeTransformer.replaceCurrent(parent, previous, node, node3);
							}
						}
					}
					goto IL_6AC;
				}
				return;
				Block_27:
				throw Context.CodeBug();
				IL_60B:
				throw Context.CodeBug();
				IL_69F:
				throw Context.CodeBug();
			}
		}
		protected internal virtual void visitNew(Node node, ScriptOrFnNode tree)
		{
		}
		protected internal virtual void visitCall(Node node, ScriptOrFnNode tree)
		{
		}
		private static Node addBeforeCurrent(Node parent, Node previous, Node current, Node toAdd)
		{
			if (previous == null)
			{
				if (current != parent.FirstChild)
				{
					Context.CodeBug();
				}
				parent.addChildToFront(toAdd);
			}
			else
			{
				if (current != previous.Next)
				{
					Context.CodeBug();
				}
				parent.addChildAfter(toAdd, previous);
			}
			return toAdd;
		}
		private static Node replaceCurrent(Node parent, Node previous, Node current, Node replacement)
		{
			if (previous == null)
			{
				if (current != parent.FirstChild)
				{
					Context.CodeBug();
				}
				parent.replaceChild(current, replacement);
			}
			else
			{
				if (previous.next == current)
				{
					parent.replaceChildAfter(previous, replacement);
				}
				else
				{
					parent.replaceChild(current, replacement);
				}
			}
			return replacement;
		}
	}
}
