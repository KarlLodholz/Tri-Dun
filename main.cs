using System;

public class Program {
    public static void Main(string[] args) {
        int width = 5;
        int height = 5;
        int[,] gen =   {{1,1,1,1,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,0,0,0,1},
                        {1,1,1,1,1}};
            
        //convert gen to a triangle map
        int[][] map = new int[height*2][];
        for(int y=0; y<height; y++) {
            map[2*y] = new int[width-(y%2==1 ? 1 : 0)];
            map[2*y+1] = new int[width-(y%2==1 ? 0 : 1)];
            for(int x=0; x<width; x++) {
                if(x!=width-1 || y%2==0)
                    map[2*y][x] = (y%2==0 ? gen[y,x] : ((gen[y,x]==1 || gen[y,x+1]==1) ? 1 : gen[y,x]));
                if(x!=width-1 || y%2!=0) 
                    map[2*y+1][x] = (y%2!=0 ? gen[y,x] : ((gen[y,x]==1 || gen[y,x+1]==1) ? 1 : gen[y,x]));
            }
        }
        // print standard map
        Console.WriteLine("reg map:");
        for(int y=0; y<gen.GetLength(0); y++) {
            string temp = "";
            for(int x=0; x<gen.GetLength(1); x++) {
                temp += gen[y,x];
                temp += " ";
            }
            Console.WriteLine(temp);
        }
        // print triangle map
        Console.WriteLine("\ntriangle converted: ");
        for(int y=0; y<map.Length; y++) {
            string temp = y%4==1||y%4==2 ? "   ":"";
            for(int x=0; x<map[y].Length; x++) {
                temp += map[y][x];
                temp += "     ";
            }
            Console.WriteLine(temp);
        }
        
    }
}