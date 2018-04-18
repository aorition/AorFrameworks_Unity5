
public enum TextPlusDataType {
    Text,
    Material
}

public class TextPlusDataMaterialInfo {

    public TextPlusDataMaterialInfo(int index, int length, int id, int size, int x, int y, int width, int height) {
        this.index = index;
        this.length = length;
        this.id = id;
        this.size = size;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public int index;

    public int length;

    public int id;

    public int size;

    public int x;

    public int y;

    public int width;

    public int height;
}

public class TextPlusDataSpriteInfo {
    public TextPlusDataSpriteInfo(int index, int length, string name, int size) {
        this.index = index;
        this.length = length;
        this.name = name;
        this.size = size;
    }

    public int index;
    public int length;
    public string name;
    public int size;

}

public class TextPlusData {

    public TextPlusData(TextPlusDataType type, string value) {
        this.type = type;
        this.value = value;
    }

    public TextPlusDataType type;

    public string value;

    public TextPlusDataMaterialInfo materialInfo;

}