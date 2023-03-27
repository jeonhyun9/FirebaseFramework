DataGenerator

Excel 파일을 기반으로, Data를 생성하는 툴입니다.

===========================================================================

1. 데이터 구조

-DataStruct
Excel 파일을 기반으로 생성된 데이터 구조체이며, IBaseData를 상속받습니다.
Excel 에서 데이터타입 변수명을 가져오고, Json으로 변환할 수 있도록 작성됩니다.

다른 Struct를 참조할 경우, 변수명은 NameId로 정의되며, DataContainer에서 해당 NameId로 찾은 데이터를 return 합니다.

-DataContainer
데이터를 담아두는 클래스이며, IBaseDataContainer를 상속받습니다.
Json 파일을 역직렬화하여 데이터 Dictionary에 저장해서 실제로 사용할 수 있도록 담아둡니다.
Dictionary로 Id, NameId로 접근하거나, 조건에 맞는 데이터를 찾아서 접근할 수 있도록 구현되어있습니다.

-DataContainerManager
DataContainer를 담아두는 싱글턴 클래스이며, BaseManager를 상속받습니다.
런타임에 실제로 사용할 데이터컨테이너를 추가할 수 있습니다.

현재는 로컬 경로에서 json을 불러오도록 구현되어있습니다.

===========================================================================

2. Editor 코드

-DataGeneratorWindow
OnGui()로 툴의 창을 그리는 클래스입니다.
Excel파일을 불러올 경로를 변경할 수 있으며, 경로는 EditorPrefs에 저장됩니다.

-DataGenerator
경로에서 Excel 파일들을 불러오고, 각 Generator에 엑셀 시트를 전달해
Data를 생성시키는 클래스입니다.
각 Generator의 변경점을 로그로 알려줍니다.

-StructGenerator
Data 구조체 스크립트를 생성하는 클래스입니다.
Excel에서 데이터타입, 변수명을 불러오고, 미리 정의된 템플릿을 기반으로
구조체의 스크립트를 생성합니다.

-ContainerGenerator
DataContainer 스크립트를 생성하는 클래스입니다.
미리 정의된 템플릿 텍스트 파일을 기반으로 DataContainer 스크립트를 생성합니다.

-JsonGenerator
엑셀 파일의 데이터들을 실제로 사용할 수 있도록 Json 파일로 변환하는 클래스입니다.
엑셀에서 데이터타입, 변수명, 값을 가져와서 변환하고 .json 파일로 저장합니다.

===========================================================================

3. 엑셀 파일 규칙

-1행
각 컬럼의 설명을 적는 부분이며, 코드에서 읽지 않는 부분입니다.

-2행
데이터 타입을 정의하는 행입니다.
enum은 enum:AnimalType
struct는 struct:DataAnimal
형식으로 작성합니다.

-3행
변수명을 정의하는 행입니다. 
enum에는 EnumDefine에 정의된 enum 값을,
struct는 해당 struct의 NameId로 작성합니다.

-4행
여기부터 실제로 저장될 데이터를 읽습니다.

===========================================================================

4. 사용법

-Excel 폴더 전체를 검사해서 데이터 생성할 경우
Tools/Generate Data From Excel
버튼을 클릭 후, Excel 파일들이 저장된 경로를 입력합니다.

-Excel 파일 하나로 데이터를 생성할 경우
해당 엑셀파일을 우클릭 후 Generated Data From Excel을 클릭합니다.

===============================================================================
