
Simple Utility Platform.

개인적으로 사용할 용도의 유틸리티들을 개발하기 위한 프로젝트.
개발 연습을 목적으로 구현되었습니다.

## Development Environments
* IDE: Visual studio 2022 CE, Jetbrains Rider
* SDK: dotNet 8.0 LTS
* Database: Postgresql 

## Simple Utility Platform Projects
#### Common Projects
* [Sup.Common](/Documents/Sup.Common.md)
* [Sup.Common.Logger](/Documents/Sup.Common.Logger.md)
#### Notion Publisher Projects
* [Sup.Np.Api](/Documents/Sup.Np.Api.md)
* [Sup.Np.IssueLoader](/Documents/Sup.Np.IssueLoader)
* [Sup.Np.PagePublisher](Sup.Np.PagePublisher.md)
* [Sup.Np.PageFixer](/Documents/Sup.Np.PageFixer.md)

## Database
postgres 도커 이미지를 사용해 구현, 개발하였습니다. 사용한 이미지에 대한 정보는 [docker-ompose.yml](/Database/docker-compose.yml)을 참고해 주시기 바랍니다.
* [ERD(DBDiagram.io)](https://dbdiagram.io/d/NotionPublisher-63bceab56afaa541e5d166c8)
* [GenerateDatabase.sql](/Database/GenerateDatabase.sql)
#### Profiles
특정 설정값의 경우 DB `profile` 테이블에 정의되어 공통으로 사용됩니다. 다음은 사전에 정의 된 프로파일의 entry와 설명입니다.
* `ES_URL`: ES 접속 정보
* `ES_USER`: ES 로그인 유저 정보
* `ES_PASSWORD`: ES 유저의 패스워드
* `REDMINE_URL`: Redmine 접속 정보
* `REDMINE_API_KEY`: Redmine API 액세스 키
* `NOTION_DB_ID`: Notion의 DB ID
* `NOTION_API_KEY`: Notion API 키.
* `NOTION_API_URL`: Notion API 접속 정보.
* `NOTION_API_VERSION`: Notion API 버전 정보.
* `LOADER_TARGET_PROJECT_IDS`: IssueLoader가 불러올 프로젝트의 ID.
* `LOADER_RECOVER_DURATION`: IssueLoader가 초기 구동 될 때 최근 몇 일 간의 이슈를 불러올지 설정.
* `FIXER_MAX_ISSUE_LIMIT`: PageFixer가 복구 시도할 Page의 Issue number 시작 값.
* `FIXER_MIN_ISSUE_LIMIT`: PageFixer가 복구 시도할 Page의 Issue number 최댓값.


## Deploy
1. postgres 도커 이미지를 실행시켜 테이블을 먼저 구성합니다.
	* ES, Notion 및 Redmine 정보는 `profile` 테이블에 삽입합니다.
2. [docker-compose.yml](/docker-compose.yml)을 수정합니다.
	* 각 제품의 환경 변수 값을 적절히 수정합니다.
3. `docker-compose up -d` 명령어로 컨테이너들을 실행 시킵니다.
