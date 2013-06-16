Intelligent Legal Document Storage System
========

MEng Final Year Project

Document storage represents a huge cost and inconvenience to most legal practices, with many storing paper copies of documents for safe keeping. Intelligent Legal Document Storage Solution (ILDSS) aims to remove this dependency on paper by digitalizing and intelligently backing up data created by legal practices.

File usage data collected from Lifetime Legal Solutions, a Yorkshire based Will writing practice, was used to investigate the idea that there are unused files which can be removed from local machines. The research showed that, for Lifetime Legal Solutions, there are a significant number of files which are unused and hence can be archived and kept in cloud storage only. This investigation resulted in the development of an algorithm, using file usage metrics collected by ILDSS. The algorithm calculates the period for which files are needed and should be retained locally before being archived and residing only in cloud storage. Further investigation would aim to prove that the algorithm calculates this period correctly.

Removing unused local files helps to increase performance and reduce disk utilization on local machines. It also helps them to play a less crucial role in securing important data and means that little or no hardware upgrades are required to implement and run the system.

ILDSS is written in C# using the .NET framework and was deployed and tested at Lifetime Legal Solutions. Cloud storage is provided by Amazon Web Services, which comprises several storage products which enable ILDSS to be competitive in terms of both cost and security.
