using Framework;


namespace Framework
{
    public class TicTacGridLogic
    {
    	int w, h, win;
    	int[,] grid;
    
    	public TicTacGridLogic(int width, int height, int winCount)
    	{
    		w = width;
    		h = height;
    		win = winCount;
    		grid = new int[w, h];
    	}
    
    	public void Set(int x, int y, int v) => grid[x, y] = v;
    
    	public bool CheckWin(int x, int y)
    	{
    		return Check(x, y, 1, 0) || Check(x, y, 0, 1) ||
    			   Check(x, y, 1, 1) || Check(x, y, 1, -1);
    	}
    
    	bool Check(int x, int y, int dx, int dy)
    	{
    		int count = 1;
    		count += Count(x, y, dx, dy);
    		count += Count(x, y, -dx, -dy);
    		return count >= win;
    	}
    
    	int Count(int x, int y, int dx, int dy)
    	{
    		int c = 0;
    		for (int i = 1; i < win; i++)
    		{
    			int nx = x + dx * i;
    			int ny = y + dy * i;
    			if (nx < 0 || ny < 0 || nx >= w || ny >= h) break;
    			if (grid[nx, ny] != 1) break;
    			c++;
    		}
    		return c;
    	}
    }
    
}