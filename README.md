SQLiteQueryBuilder
==================

It’s an amazing, free and open source .net library to get rid of repetitive queries writing.

WHAT IT ISN'T:
--------------

This is NOT an ORM framework nor database connection manager. All it do is compose sql query strings

What is it?
-----------
,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,

It’s a simple tool that use reflection to dynamically provide strings with SQLite statements using
the C\# object notation, what making **DAL** maintenance pretty easy and avoid a lot of
dangerous `Ctrl + C`, `Ctrl + V` based changes on strings annoying concatenations.

The library was designed to be basic enough to do the job but allowing you to
create elaborated solutions according to your project's architecture.

### **Targets**

Wherever you have the need to build SQLite query strings it would fit.
 
### **Features**

On the following board, you can see the library’s features plan. We’re glad to
receive your suggestions about what more it can do to make our development cool
and amazing.

| **Feature** | **Available** | **Library version** | **Notes** |
|:-----------:|:-------------:|:-------------------:|:---------:|
| Select query | ![Available][ok] | 0.0.1.0+ | Simple, Distinct, Top, Count and custom |
| Join | ![Available][ok] | 0.0.1.0+ | Inner and Outer |
| Single Conditions | ![Available][ok] | 0.0.1.0+ | |
| Block conditions | ![Available][ok] | 0.0.1.0+ | |
| Operators support | ![Available][ok] | 0.0.1.0+ | `OR,` `AND,` `LIKE,` `IN`, `NOT IN`, `>=`, `<=`, `>`, `<` and `<>` |
| Custom column select | ![Available][ok] | 0.0.1.0+ | |
| Union | ![Available][ok] | 0.0.1.5+ | |
| Lambda Expressions support on conditions | ![Developing][coding] | | Less parameters to query construction |
| Inferred joins* | ![Developing][coding] | | Just pass table types as generic parameters |
| `Exists` and `Not Exists` operators | ![Developing][coding] | | |
| Group By | ![Soon][soon] | | |
| Methods for return expression trees | ![Soon][soon] | | |
| Class x Table and Property x Column mapping | ![Soon][soon] | | |
| Custom conditions | ![Unavailable][nok] | | |
| Custom joins | ![Unavailable][nok] | | |
| Order By | ![Unavailable][nok] | | |

\* _Just for single keyed tables with attributes `PrimaryKey `and `ForeignKey` defined_

Why can it be useful to you?
---------------------------

### Clean, Reusable and Maintenable

```C#
string param = "not";
string[] filter = new [] { "easy", "quick"};

string phrase = "SELECT 'It is " + param + " just a SQL trauma' as [MainReason] ";
phrase += "FROM Too_Many_Years_Facing_Cumbersome_Crazyness_Like_This as table ";
phrase += " WHERE table.Maintenance " + param + " IN + ("  + string.Join(", ", filter) + ") ";
```

* Enough with headaches wasting hours seeking for a single character messing the things up after just to add a column. Your time deserves more
* There's no reasonable reason to keep thousands of classes to make the same queries where almost only columns and tables names are changed
* You get much more performance than using the common built-in lambda based feature (they loads the whole table data to memory to perform filters)

Coding without handle string keep the code dry and clean, allowing you to create reusable custom features to your data access strategies and makes the code fluent and pretty easy to understand and maintain.

How to use
----------

Let's see some code!

Supposing a simple class (database table equivallent):

```C#
public class Person
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public int Age { get { return Convert.ToInt32((DateTime.Now.Date - BirthDate.Date).TotalDays / 365); } }

    /*
       The `Age` property is read only, so it will be ignored by QueryBuilder on SELECT clause.
       It's the same for that one with `Ignore` or relationships attribute, like these:

       [Ignore]
       public int Age { get; set; }
       [ManyToOne]
       public Person Father { get; set; }
    */
}
```

### Simple, distinct, count and top queries for persons born before Oct 14, 1995

Easy and quick! Just like this:

```C#
QueryBuilder qb = new QueryBuilder<Person>();
var filter = new Condition<Person, DateTime>(nameof(Person.BirthDate), BoolComparisonType.LessThan, new DateTime(1995, 10, 14));
qb.AddWhereCondition(filter);

string qCount = qb.BuildCount();
string qDistinct = qb.BuildSelectDistinct();
string qTop = qb.BuildSelectTop(7);
```

**The results are:**
```sql
SELECT TOP 7 Person.Id, Person.Name, Person.BirthDate -- or 'SELECT COUNT(*) (...)' , 'SELECT DISTINCT Person.Id, (...)' or even 'SELECT Person.Id, (...)'
FROM Person Person 
WHERE  Person.BirthDate < '1995-10-14 00:00:00' 
```

### Joins and Condition Blocks

To exemplify joins, lets include a new property on `Person` and create `Dog` and `Cat` classes:

```C#
public class Person
{
    (...) // As we set before
	
    [OneToMany] // As it's a relationship, it'll be ignored on query
    public List<Dog> Pets { get; set; }
}

public class Dog
{
    public string Name { get; set; }
    // A person id reference
    public long IdOwner { get; set; }
    public string FavoriteFood { get; set; }
    public double Weight { get; set; }

    [ManyToOne] // As it's a relationship, it'll be ignored on query
    public Person Owner { get; set; } 

    public override void Speak()
    {
	System.Diagnostics.Debug.WriteLine("Rooff!");
    }
}
```

Now, lets take persons with the previous filter conditions and additionally have a dog with one of the names of the list [ "Spike", "Thor" ] **OR** the dog's `FavoriteFood` have the word "fruit" **AND** weighs less than 7.5kg.

Just add this statements to the previous query:

```C#
var joinCondition = new Condition<Person, Dog>(nameof(Person.Id), BoolComparisonType.Equals, nameof(Dog.IdOwner));

qb.AddJoin<Person, Dog>(JoinType.Inner /* Join type */, null /* alias for the right type table */, joinCondition);

var singleCondition = new Condition<Dog, double>(nameof(Dog.Weight) /* Left table property name */, BoolComparisonType.LessThanOrEqualTo /* Operator */, 7.5 /* raw value or right table property */);

var innerCondOne = new Condition<Dog, string>(nameof(Dog.Name), BoolComparisonType.In, new object[] { "Spike", "Thor" }, LogicalOperatorType.Or /* logical operator for condition's concatenation */);
var innerCondTwo = new Condition<Dog, string>(nameof(Dog.FavoriteFood), BoolComparisonType.Like, "%fruit%");

var block = new ExpressionBlock(LogicalOperatorType.And /* logical operator for condition's concatenation */, innerCondOne, innerCondTwo ),

qb.AddWhereCondition(block, singleCondition);
```

Result:

```sql
SELECT TOP 7 Person.Id, Person.Name, Person.BirthDate 
FROM Person Person 
    JOIN Dog Dog ON  Person.Id = Dog.IdOwner  
WHERE  Person.BirthDate < '1995-10-14 00:00:00'  AND 
(  Dog.Name IN ( 'Spike', 'Thor' )  OR 
 Dog.FavoriteFood LIKE '%fruit%'  )
 AND 
 Dog.Weight <= 7.5 
```

Contribution, bug report and questions
--------------------------------------

Contributions are welcome! Feel free to report if you find a bug or if you want a new feature or give any kind of suggestions.

You can find me on StackOverflow if you have questions related to the use of `SQLiteQueryBuilder`.

***Have a nice coding!***

[ok]: https://raw.githubusercontent.com/diegorafael/resources/master/images/icons8-ok-24.png
[nok]: https://raw.githubusercontent.com/diegorafael/resources/master/images/icons8-cancel-24.png
[soon]: https://raw.githubusercontent.com/diegorafael/resources/master/images/icons8-inactive-state-24.png
[coding]: https://raw.githubusercontent.com/diegorafael/resources/master/images/icons8-source-code-24.png
