
SUP에서 공통적으로 사용하기 위한 클래스 라이브러리.

---

## [Consts.cs](../Sup.Common/Consts.cs)

여러 제품에서 공통으로 사용하기 위한 상수값들의 모음.


## [Configs](../Sup.Common/Configs)

여러 제품에서 공통으로 사용하기 위한 설정을 정의한 클래스들의 모음.


## [Entities](../Sup.Common/Entities)

DB 엔티티들을 정의한 클래스들의 모음.


## [Exceptions](../Sup.Common/Exceptions)

여러 제품에서 사용하기 위한 사용자 정의 예외 클래스들의 모음.


## [Models](../Sup.Common/Models)

여러 제품에서 데이터 전송 및 처리를 위한 데이터 모델 클래스들의 모음.
* ### Notion
	* Notion DB의 Page 구성이 정의되어 있는 클래스.
	* Notion Api [Retrieve Page](https://developers.notion.com/reference/retrieve-a-page)의 Json 응답을 클래스로 구성하였습니다.
* ### Redmine
	* Redmine의 Project 및 Issue 구성의 정의되어 있는 클래스
	* Redmine Api [Listing projects](https://www.redmine.org/projects/redmine/wiki/Rest_Projects)와 [Listing issues](https://www.redmine.org/projects/redmine/wiki/Rest_Issues)의 응답을 클래스로 구성하였습니다. 
* ### RequestParams
	* API 요청 시 사용되는 파라미터 클래스.
* ### Response
	* API 응답 시 사용되는 클래스.

## [Utils](../Sup.Common/Utils)

여러 제품에서 공통으로 사용하기 위한 유틸리티 클래스들의 모음.

* ### Encrypters
	* AES-128 기반의 암호화 및 복호화 기능을 제공하는 유틸리티.
	* 편의 상 key 값만 입력 받아 동작 되도록 구성 되었습니다. 만약 IV값을 바꾸고 싶으면 내부의 값을 직접 수정해야 합니다.

