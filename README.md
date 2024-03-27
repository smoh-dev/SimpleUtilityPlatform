
Simple Utility Platform.

개인적으로 사용할 용도의 유틸리티들을 개발하기 위한 프로젝트.
개발 연습을 목적으로 구현되었습니다.

#### Development Environments
* IDE: Visual studio 2022 CE, Jetbrains Rider
* SDK: dotNet 8.0 LTS
* Database: Postgresql 

#### Projects
* [Sup.Common](/Documents/Sup.Common.md)
* [Sup.Common.Logger](/Documents/Sup.Common.Logger.md)
* [Sup.Np.Api](/Documents/Sup.Np.Api.md)
* [Sup.Np.IssueLoader](/Documents/Sup.Np.IssueLoader)
* [Sup.Np.PageFixer](/Documents/Sup.Np.PageFixer.md)

#### Database
postgres 도커 이미지를 사용해 구현, 개발하였습니다. 사용한 이미지에 대한 정보는 [docker-ompose.yml](/Database/docker-compose.yml)을 참고해 주시기 바랍니다.
* [ERD(DBDiagram.io)](https://dbdiagram.io/d/NotionPublisher-63bceab56afaa541e5d166c8)
* [GenerateDatabase.sql](/Database/GenerateDatabase.sql)
