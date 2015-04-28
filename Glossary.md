# Introduction #
Flex Time Table was developed initially for Walter Sisulu University by the ICT Director, Mr Courtney Walker.  The terms used are therefore associated with the University.


  * [Academic Block](#Academic_Block.md)
  * [Campus](#Campus.md)
  * [Class](#Class.md)
  * [Class Group](#Class_Group.md)
  * [Cluster](#Cluster.md)
  * [Offering Type](#Offering_Type.md)
  * [Program](#Program.md)
  * [Qualification](#Qualification.md)
  * [Resource](#Resource.md)
  * [Site](#Site.md)
  * [Subject](#Subject.md)

## Academic Block ##
The Academic Block defines a block of weeks within an academic year that a particular [Class](#Class.md) is held.  It is defined by the start week and end week. Normally a subject it is defined as Full Academic Year or Semester Subject. The system allows flexibility by defining Academic blocks according a start week and an end week. Example:

|**ID**|**Name**|**Start**|**End**|**Year**|
|:-----|:-------|:--------|:------|:-------|
|1 |Year|1 |52|Yes|
|2 |Semester 1|1 |36|No|
|3 |Semester 2|37|52|No|

[Goto Top](#Introduction.md)
## Campus ##
For most universities a campus is a large private area with many buildings. At some universities such as Walter Sisulu University(WSU) a campus is a virtual area with many sites. In the case of WSU a campus encompasses a particular municipal area such as Buffalo City, Butter worth, Mthatha and Queenstown. In some cases the sites within a campus are separated by up to 30Km.

[Goto Top](#Introduction.md)
## Class ##
A class is similar to a [Class Group](#Class_Group.md).

[Goto Top](#Introduction.md)
## Class Group ##
A Class Group defines a particular class that is taught by a particular lecturer at a particular [Cluster](#Cluster.md) for a specific subject and for selected students.

[Goto Top](#Introduction.md)
## Cluster ##
In cases where a campus has sites separated by fairly long distances it is necessary to cluster sites together so that a program can have classes shared across sites within a particular cluster. Normally the sites are separated by less than 1Km and allows lecturers and students to travel effortlessly between the sites in the cluster. A large site that has no adjacent sites can be established as a cluster on its own.

[Goto Top](#Introduction.md)
## Offering Type ##
The Offering Type determines the daily time period for a class. It also determines whether the class can held on weekend. The Offering Type normally determines whether a program is full time or Part time.  Full Time is normally held during the day and Part Time usually held after normal working hours.

|ID|	Code|	Name|	Start	|End|	Sat. Classes	|Start	|End|
|:-|:----|:----|:------|:--|:-------------|:-----|:--|
|1 | Full Time |Full Time|08:00|16:25|No| 	08:00 |	15:35|

[Goto Top](#Introduction.md)
## Program ##
This is essentially the particular course of study that a student is undertaking at the university.

[Goto Top](#Introduction.md)
## Qualification ##
This is the same as program.

[Goto Top](#Introduction.md)
## Resource ##
A Resource is associated with a class.  It can be lecture, a lab or a group tutorial.  The application gives flexibility in creating any number of resources for a particular class.   In some cases a class can have different labs for different groups of students within the same class.

[Goto Top](#Introduction.md)
## Site ##
A site is particular branch of the University. It is a part of a campus as well as a part of a cluster and some cases it can constitute a cluster.

[Goto Top](#Introduction.md)
## Subject ##
Wikipedia gives a comprehensive definition at http://en.wikipedia.org/wiki/Course_%28education%29.

[Goto Top](#Introduction.md)