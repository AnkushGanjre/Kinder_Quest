using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "SOFiles/DataSO")]
public class BaseDataSO : ScriptableObject
{
    [Header("Alphabet & Number List")]
    public string[] NumberList = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" };
    public string[] AllAlphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    public string[] AlphabetList = { "Apple", "Ball", "Cat", "Dog", "Egg", "Fish", "Grape", "Horse", "Ice_Cream", "Jar", "Kite", "Lion", "Moon", "Noodle",
                                     "Orange", "Pie", "Queen", "Rabbit", "Sun", "Tiger", "Unicorn", "Van", "Whale", "Xylophone", "Yak", "Zebra"};

    [Header("Words Array")]
    public string[] FoodList = { "Pizza", "Burger", "Pasta", "Candy", "Donut", "Bread", "Fries", "Chips", "Cake", "Pie", "Cookie", "Noodle" };
    public string[] FruitList = { "Apple", "Mango", "Grape", "Guava", "Peach", "Kiwi", "Pear", "Banana", "Oranges", "Berry", "Cherry", "Papaya" };
    public string[] AnimalList = { "Dog", "Cat", "Hen", "Bear", "Deer", "Lion", "Tiger", "Cow", "Horse", "Wolf", "Yak", "Zebra" };
    public string[] ExtrasList = { "Ball", "Balloons", "Chess", "Jar", "Kite", "Moon", "Queen", "Star_Emoji", "Sun", "Tablet", "Traffic_Cone", "Van", "Xylophone" };

    [Header("Colors Array")]
    public string[] ColorList = { "Red", "Blue", "Green", "Yellow", "Pink", "Black", "White", "Gray", "Brown", "Orange", "Silver", "Purple" };
    public string[] HexCodeList = { "#FF0000", "#0000FF", "#00FF00", "#FFFF00", "#e75480", "#000000", "#FFFFFF", "#A9A9A9", "#964B00", "#FFA500", "#C0C0C0", "#800080" };

    [Header("Shapes Array")]
    public string[] ShapeList = { "Circle", "Square", "Triangle", "Rectangle", "Star", "Diamond", "Heart", "Oval"};
    public string[] ShapeObjList = { "Ball", "Chess", "Traffic_Cone", "Tablet", "Star_Emoji", "Kite", "Balloons", "Egg" };
    public string[] ColorObjList = { "Cherry", "Whale", "Pear", "Mango", "Peach", "Zebra", "Egg", "Tablet", "Bear", "Oranges", "Moon", "Berry" };
}
