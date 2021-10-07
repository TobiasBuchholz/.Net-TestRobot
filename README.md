# .Net-TestRobot

<img align="right" width="100" height="100" src="https://raw.githubusercontent.com/TobiasBuchholz/.Net-TestRobot/master/art/icon.png">

TestRobot is a small library to facilitate writing more stable, readable and maintainable tests by applying the Robot Pattern.

## What is the Robot Pattern?

The main idea behind the Robot Pattern is to hide the implementation details of tests that are following the AAA Pattern (Arrange-Act-Assert) in a so called Robot and RobotResult class. Combining this idea with the Builder Pattern makes it easy to write very readable and maintainable tests that could look like the following:

```c#
[Theory]
[InlineData(42, true)]
[InlineData(0, false)]
public void catches_pokemon_regarding_to_pokeball_amount(int pokeBallAmount, bool expectedIsPokemonCaught)
{
    new PokeTrainerRobot()
        // arrange
        .WithIsPokemonDetectable(true)
        .WithPokeBallAmount(pokeBallAmount)
        .Build()
        // act
        .CatchPokemonAsync()
        .AdvanceUntilEmpty()
        // assert
        .AssertIsPokemonCaught(expectedIsPokemonCaught)
        .VerifyPokeDexMock(x => x.DetectPokemonAsync())
        .WasCalledExactlyOnce()
        .VerifyInventoryMock(x => x.GetPokeBallCount())
        .WasCalledExactlyOnce()
        .VerifyInventoryMock(x => x.UsePokeBallAsync())
        .WasCalledExactly(expectedIsPokemonCaught ? 1 : 0);
}

```
For more information take a look at this [github gist](https://gist.github.com/bharatdodeja/ac001b6a24028bde56943ee40cab7dbd) which offers a quite good explanation for the Robot Pattern in the context of Android Esspresso UI Testing.

## Installation
#### Nuget
[![NuGet](https://img.shields.io/nuget/v/TestRobot.svg?maxAge=86400&style=flat)](https://www.nuget.org/packages/TestRobot/1.0.0)
> Install-Package TestRobot

[![NuGet](https://img.shields.io/nuget/v/TestRobot.CodeGenerator.svg?maxAge=86400&style=flat)](https://www.nuget.org/packages/TestRobot.CodeGenerator/1.0.0)

> Install-Package TestRobot.CodeGenerator

## Dependencies
###### [PCLMock](https://github.com/kentcb/PCLMock)
A lightweight, but powerful mocking framework by Kent Boogaart.

###### [Genesis.TestUtil](https://github.com/kentcb/Genesis.TestUtil)
A simple library containing helpers for test code also by Kent Boogaart. The main reason for using it is it's `TestScheduler` class, which is a virtual time scheduler using [Reactive Extensions](https://github.com/dotnet/reactive) and makes it possible to control time in your tests.

###### ([TestRobot.CodeGenerator](https://www.nuget.org/packages/TestRobot.CodeGenerator/1.0.0))
This additional library is not needed necessarily but it generates some boiler plate code for you, hence I'd recommend to use it. It generates the `AutoTestRobot` and `AutoTestRobotResult` classes that you will see in the example code below. These classes serve as the base classes for the Robot Pattern. The code generator scans your test project's files for classes that are ending with *Mock and generates instances of these mocks in the `AutoTestRobot` class, so they can be used for the creation of the System Under Test (SUT) object. Furthermore it generates methods that help to verify times of method executions by the generated mocks in the `AutoTestRobotResult` class.

## Disclaimer
I'm aware of the library's tightly coupling to the Mocking Framework [PCLMock](https://github.com/kentcb/PCLMock) and [Reactive Extensions](https://github.com/dotnet/reactive), which could be reasons for not using it, especially in projects that already have a test environment established. I've developed the library for my own particular needs but I thought it might be worth sharing it with the community. If there are suggestions for improvement I would be very happy to receive them.

## Sample code
In order to explain how to use this library I've created a small set of simple classes that represent some arbitrary business logic:

- [Inventory.cs](https://github.com/TobiasBuchholz/.Net-TestRobot/blob/main/src/Playground/Features/Inventory.cs) (extends [IInventory.cs](https://github.com/TobiasBuchholz/.Net-TestRobot/blob/main/src/Playground/Features/IInventory.cs))
- [PokeDex.cs](https://github.com/TobiasBuchholz/.Net-TestRobot/blob/main/src/Playground/Features/PokeDex.cs) (extends [IPokeDex.cs](https://github.com/TobiasBuchholz/.Net-TestRobot/blob/main/src/Playground/Features/IPokeDex.cs))
- [PokeTrainer.cs](https://github.com/TobiasBuchholz/.Net-TestRobot/blob/main/src/Playground/Features/PokeTrainer.cs)

The `PokeTrainer` class takes instances of `IInventory` and `IPokeDex` as constructor parameters, provides the methods `CatchPokemonAsync()` and `HealPokemonsAsync()` and will serve as the System Under Test (SUT) in the example below.

## How to use the library

As mentioned in the short description above this library tries to facilitate writing tests that are following the AAA pattern, so this How-To will be structured into three parts:
1. [Arrange](#arrange)
2. [Act](#act)
3. [Assert](#assert)

### 1. Arrange

The first step is to create your own TestRobot and TestRobotResult classes by extending the `AutoTestRobot` and `AutoTestRobotResult` classes and implement their abstract methods. The TestRobotResult is the second ingredient for the Robot Pattern and is responsible for the assertion and verification of the tests' outcome. Both base classes get generated automagically by the [TestRobot.CodeGenerator](https://www.nuget.org/packages/TestRobot.CodeGenerator/1.0.0) package (see description in section **Dependencies** above):

```c#
public sealed class PokeTrainerRobot : AutoTestRobot<PokeTrainer, PokeTrainerRobot, PokeTrainerRobotResult>
{
    protected override PokeTrainer CreateSut(TestScheduler scheduler)
    {
      // see the implementation below
    }
}

public sealed class PokeTrainerRobotResult : AutoTestRobotResult<PokeTrainer, PokeTrainerRobot, PokeTrainerRobotResult>
{
    public PokeTrainerRobotResult(PokeTrainer sut, PokeTrainerRobot robot)
        : base(sut, robot)
    {
    }
}
```

The next step is to implement the `CreateSut()` method of the TestRobot class. This method creates an instance of the System Under Test (SUT), so the class you are aiming to write your tests against:

```c#
// in PokeTrainerRobot.cs

...

protected override PokeTrainer CreateSut()
{
    return new PokeTrainer(
        InventoryMock,
        PokeDexMock,
        TestScheduler);
}

...
```

`InventoryMock` and `PokeDexMock` are properties of the `AutoTestRobot` base class that are generated automagically by the [TestRobot.CodeGenerator](https://www.nuget.org/packages/TestRobot.CodeGenerator/1.0.0) package. If needed you can override the also automagically generated creation methods of the mocks in your TestRobot class to adapt their behaviour to your needs:

```c#
// in PokeTrainerRobot.cs

...

protected override InventoryMock CreateInventoryMock()
{
    var mock = base.CreateInventoryMock();
    mock.When(x => x.GetPokeBallCount()).Return(_pokeBallAmount);
    return mock;
}

protected override PokeDexMock CreatePokeDexMock()
{
    var mock = base.CreatePokeDexMock();
    mock.When(x => x.DetectPokemonAsync()).ReturnsAsync(_isPokemonDetectable);
    return mock;
}

...
```

To insert parameters into your TestRobot that should be returned by the mocks, e.g. the amount of PokeBalls contained in the inventory, use the `With()` method provided by the `TestRobotBase` class:

```c#
// in PokeTrainerRobot.cs

...

public PokeTrainerRobot WithIsPokemonDetectable(bool isDetectable) =>
    With(ref _isPokemonDetectable, isDetectable);

public PokeTrainerRobot WithPokeBallAmount(int amount) =>
    With(ref _pokeBallAmount, amount);

...
```

Now everything is prepared to write the Arrange part of the AAA Pattern in a beautiful manner:

```c#
// in PokeTrainerFixture.cs

[Fact]
public void catches_pokemon_when_has_enough_pokeballs()
{
    new PokeTrainerRobot()
        .WithIsPokemonDetectable(true)
        .WithPokeBallAmount(42)
        .Build()
    ...
}
```

### 2. Act
After the preparation is done, it's time to act. To invoke the business logic of your SUT object simply write a method in your TestRobot to invoke the desired method and let it return your TestRobot class so the methods can also be chained:

```c#
// in PokeTrainerRobot.cs

...

public PokeTrainerRobot CatchPokemonAsync()
{
    Schedule(() => _sut.CatchPokemonAsync());
    return this;
}

...
```

`Schedule()` is a protected method of the `TestRobotBase` class that executes the given action on a `TestScheduler`. This implementation of `IScheduler` is a virtual time scheduler using [Reactive Extensions](https://github.com/dotnet/reactive) and is provided by the [Genesis.TestUtil](https://github.com/kentcb/Genesis.TestUtil) package. This scheduler makes it possible to let your tests travel trough time. For that matter there is another overload of the `Schedule()` method that accepts a `TimeSpan` as time after which the action should be executed:

```c#
// in PokeTrainerRobot.cs

...

public PokeTrainerRobot HealPokemonsAsync(long atTime = 0)
{
    Schedule(TimeSpan.FromMilliseconds(atTime), () => _sut.HealPokemonsAsync());
    return this;
}

...
```

After all desired actions for the test case have been scheduled the last thing to do is to call the `AdvanceUntilEmpty()` method, that is implemented in the `TestRobotBase` class. It will run the code on the `TestScheduler` and execute all the scheduled actions:

```c#
// in PokeTrainerFixture.cs

[Fact]
public void catches_pokemon_when_has_enough_pokeballs()
{
    new PokeTrainerRobot()
        .WithIsPokemonDetectable(true)
        .WithPokeBallAmount(42)
        .Build()
        .CatchPokemonAsync()
        .AdvanceUntilEmpty()
    ...
}
```

### 3. Assert

After the business logic has been executed, it's now time to assert the result. For that matter you'll need to add an assertion method to your TestRobotResult class:

```c#
// in PokeTrainerRobotResult.cs

...

public PokeTrainerRobotResult AssertIsPokemonCaught(bool expected)
{
    Assert.Equal(expected, Robot._sut.IsPokemonCaught);
    return this;
}

...
```
In the TestRobotResult class you have access to the instance of the TestRobot, meaning you can also access the SUT to assert it's properties. The assertion method again should return the TestRobotResult class so the methods can be chained.

In addition to assertion you can also verify whether the injected dependencies of your SUT have got executed the expected times by calling the automagically created verification methods of the `AutoTestRobotResult` class:

```c#
// in PokeTrainerFixture.cs

[Fact]
public void catches_pokemon_when_has_enough_pokeballs()
{
    new PokeTrainerRobot()
        .WithIsPokemonDetectable(true)
        .WithPokeBallAmount(42)
        .Build()
        .CatchPokemonAsync()
        .AdvanceUntilEmpty()
        .AssertIsPokemonCaught(true))
        .VerifyPokeDexMock(x => x.DetectPokemonAsync())
        .WasCalledExactlyOnce()
        .VerifyInventoryMock(x => x.GetPokeBallCount())
        .WasCalledExactlyOnce()
        .VerifyInventoryMock(x => x.UsePokeBallAsync())
        .WasCalledExactlyOnce();
}
```

That's it! The first steps to more readable and maintainable tests are done.

## Bonus: Traveling through time
As mentioned above this library makes it easy to control time inside your testing code. To show off this feature the `PokeTrainer` class contains a second method `HealPokemonsAsync()`. This method uses every second a healing potion for all the Pokemons that are contained in the inventory:

```c#
// in PokeTrainer.cs

...

public Task HealPokemonsAsync()
{
    Logger.Log("Healing all Pokemons of inventory");

    return Observable
        .Interval(TimeSpan.FromSeconds(1), _scheduler)
        .Take(_inventory.GetPokemonCount())
        .SelectMany(_ => _inventory.UseHealingPotionAsync().ToObservable())
        .Do(_ => Logger.Log("Pokemon was healed"))
        .ToTask();
}

...
```

The class member `_scheduler` gets injected into the `PokeTrainer` class as constructor parameter and is of type `IScheduler`. As shown in section **1. Arrange** the SUT object gets created with the `TestScheduler` of the `TestRobotBase` class as third constructor parameter. That means you can now use another method of the `TestRobotBase` class called `AdvanceTo(TimeSpan time)` to travel through time:

```c#
// in PokeTrainerFixture.cs

[Fact]
public void heals_pokemons_of_inventory()
{
    var robot = new PokeTrainerRobot()
        .WithPokemonCount(6)
        .Build()
        .HealPokemonsAsync(atTime:250);

    robot
        .AdvanceTo(TimeSpan.FromMilliseconds(100))
        .VerifyInventoryMock(x => x.UseHealingPotionAsync())
        .WasNotCalled();

    robot
        .AdvanceTo(TimeSpan.FromMilliseconds(1500))
        .VerifyInventoryMock(x => x.UseHealingPotionAsync())
        .WasCalledExactlyOnce();

    robot
        .AdvanceTo(TimeSpan.FromMilliseconds(4500))
        .VerifyInventoryMock(x => x.UseHealingPotionAsync())
        .WasCalledExactly(4);

    robot
        .AdvanceUntilEmpty()
        .VerifyInventoryMock(x => x.UseHealingPotionAsync())
        .WasCalledExactly(6);
}
```
-----------------------------------------
<div>*Icon erstellt von <a href="https://www.flaticon.com/de/autoren/itim2101" title="itim2101">itim2101</a></div>