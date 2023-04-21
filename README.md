1. 엑셀 파일 규칙

1.1) 1행
각 컬럼의 설명을 적는 부분이며, 코드에서 읽지 않는 부분입니다.

1.2) 2행
데이터 타입을 정의하는 행입니다.
enum은 enum:AnimalType
struct는 struct:DataAnimal
형식으로 작성합니다.

1.3) 3행
변수명을 정의하는 행입니다. 
enum에는 EnumDefine에 정의된 enum 값을,
struct는 해당 struct의 NameId로 작성합니다.

1.4) 4행
여기부터 실제로 저장될 데이터를 읽습니다.

===========================================================================

2. 사용법

2.1) 데이터 생성
Tools/Generate Data From Excel

2.2) 데이터 업로드
Tools/Upload Data to Firebase Storage

2.3) 스크립트 생성
Tools/Generate Script


===============================================================================
