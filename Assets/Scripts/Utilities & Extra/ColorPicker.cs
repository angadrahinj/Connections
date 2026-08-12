using UnityEngine;

public static class ColorPicker
{
    public static Color CATEGORY_YELLOW_COLOR = "#F9DF6E".FromHex();
    public static Color CATEGORY_GREEN_COLOR = "#9FC558".FromHex();
    public static Color CATEGORY_BLUE_COLOR = "#B1C4EF".FromHex();
    public static Color CATEGORY_PURPLE_COLOR = "#BA81C5".FromHex();

    public static Color GetCategoryColor(Category category)
    {
        return category switch
        {
            Category.Yellow => CATEGORY_YELLOW_COLOR,
            Category.Green => CATEGORY_GREEN_COLOR,
            Category.Blue => CATEGORY_BLUE_COLOR,
            Category.Purple => CATEGORY_PURPLE_COLOR,
            _ => Color.white
        };
    }
}
