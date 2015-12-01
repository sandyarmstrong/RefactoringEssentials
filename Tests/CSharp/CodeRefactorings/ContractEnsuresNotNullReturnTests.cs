using System;
using NUnit.Framework;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using RefactoringEssentials.CSharp.CodeRefactorings;
using System.Diagnostics.Contracts;

namespace RefactoringEssentials.Tests.CSharp.CodeRefactorings
{
    [TestFixture]
    public class ContractEnsuresNotNullReturnTests : CSharpCodeRefactoringTestBase
    {
        [Test]
        public void ValueTypeReturnType()
        {
            TestWrongContext<ContractEnsuresNotNullReturnCodeRefactoringProvider>(
                @"class Test
            {
                public $int Foo() 
                { 
                }
            }");
        }

        [Test]
        public void NullableValueTypeLocalVariable()
        {
            TestWrongContext<ContractEnsuresNotNullReturnCodeRefactoringProvider>(
                @"class Test
            {
                public void Foo() 
                { 
                    $int? foo;
                }
            }");
        }

        [Test]
        // Not sure why anyone would want this when can just change the return type to be non nullable. Maybe you have to implement an interface you don't control or something strange like that.
        public void NullableValueTypeReturnType()
        {
            Test<ContractEnsuresNotNullReturnCodeRefactoringProvider>(
                @"class Test
{
    $public int? Foo() 
    { 
    }
}", @"using System.Diagnostics.Contracts;

class Test
{
    public int? Foo() 
    {
        Contract.Ensures(Contract.Result<int?>() != null);
    }
}");
        }

        [Test]
        public void ObjectReturnType()
        {
            Test<ContractEnsuresNotNullReturnCodeRefactoringProvider>(
                @"class Test
{
    $public Cedd Foo() 
    { 
    }
}", @"using System.Diagnostics.Contracts;

class Test
{
    public Cedd Foo() 
    {
        Contract.Ensures(Contract.Result<Cedd>() != null);
    }
}");
        }

        [Test]
        public void TestUsingStatementAlreadyThere()
        {
            Test<ContractEnsuresNotNullReturnCodeRefactoringProvider>(
                @"using System.Diagnostics.Contracts;

class Test
{
    $public Cedd Foo() 
    { 
    }
}", @"using System.Diagnostics.Contracts;

class Test
{
    public Cedd Foo() 
    {
        Contract.Ensures(Contract.Result<Cedd>() != null);
    }
}");
        }

        [Test]
        public void TestContractEnsuresReturnAlreadyThere()
        {
            TestWrongContext<ContractEnsuresNotNullReturnCodeRefactoringProvider>(@"class Test
{
    public $Cedd Foo() 
    {
        Contract.Ensures(Contract.Result<Cedd>() != null);
    }
}");
        }

        [Test]
        public void TestContractEnsuresReturnAlreadyThereWithWhitespace()
        {
            TestWrongContext<ContractEnsuresNotNullReturnCodeRefactoringProvider>(@"class Test
{
    public $Cedd Foo() 
    {
        Contract.Ensures(Contract.Result<Cedd>() !=     null);
    }
}");
        }

        [Test]
        public void TestContractEnsuresReturnAlreadyThereReversedParameters()
        {
            TestWrongContext<ContractEnsuresNotNullReturnCodeRefactoringProvider>(@"class Test
{
    public $Cedd Foo() 
    {
        Contract.Ensures(null != Contract.Result<Cedd>());
    }
}");
        }

        [Test]
        public void Test_OldCSharp()
        {
            var parseOptions = new CSharpParseOptions(
                LanguageVersion.CSharp5,
                DocumentationMode.Diagnose | DocumentationMode.Parse,
                SourceCodeKind.Regular,
                ImmutableArray.Create("DEBUG", "TEST")
            );

            Test<ContractEnsuresNotNullReturnCodeRefactoringProvider>(@"class Foo
{
    Cedd $Test ()
    {
    }
}", @"using System.Diagnostics.Contracts;

class Foo
{
    Cedd Test ()
    {
        Contract.Ensures(Contract.Result<Cedd>() != null);
    }
}", parseOptions: parseOptions);
        }

    }
}
