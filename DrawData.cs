using System;

abstract class DrawElement{
	public abstract int getWidth();
	public abstract string[] getDrawString(int width);
}

class Branch : DrawElement{
	public override int getWidth(){
		return 1;
	}
	
	public override string[] getDrawString(int width){
		string[] s = new string[3];
		int midpos = (width-1)/2;
		s[0] = new string(' ', midpos);
		s[0] += '│';
		s[0] += new string(' ', width - midpos - 1);
		s[1] = s[0];
		s[2] = s[0];
		
		return s;
	}
	
	public override string ToString(){
		return "B";
	}
}

class Split : DrawElement{
	public bool corner = false;
	public bool left = false;
	public bool right = false;
	
	public override int getWidth(){
		return 1;
	}
	
	public override string[] getDrawString(int width){
		string[] s = new string[3];
		int midpos = (width-1)/2;
		s[0] = new string(' ', midpos);
		s[0] += '│';
		s[0] += new string(' ', width - midpos - 1);
		
		s[1] = "";
		if(left){
			s[1] += new string('─', midpos);
		}else{
			s[1] += new string(' ', midpos);
		}
		
		if(left && right && corner){
			s[1] += '┼';
		}else if(left && right){
			s[1] += '┴';
		}else if(left && corner){
			s[1] += '┤';
		}else if(left){
			s[1] += '┘';
		}else if(right && corner){
			s[1] += '├';
		}else if(right){
			s[1] += '└';
		}else if(corner){
			s[1] += '│';
		}else{
			s[1] += '?';
		}
		
		if(right){
			s[1] += new string('─', width - midpos - 1);
		}else{
			s[1] += new string(' ', width - midpos - 1);
		}
		
		if(corner){
			s[2] = new string(' ', midpos);
			s[2] += '│';
			s[2] += new string(' ', width - midpos - 1);
		}else{
			s[2] = new string(' ', width);
		}
		
		return s;
	}
	
	public override string ToString(){
		return "S" + (corner ? "v" : "");
	}
}

class Merge : DrawElement{
	public bool corner = false;
	public bool left = false;
	public bool right = false;
	
	public override int getWidth(){
		return 1;
	}
	
	public override string[] getDrawString(int width){
		string[] s = new string[3];
		int midpos = (width-1)/2;
		
		if(corner){
			s[0] = new string(' ', midpos);
			s[0] += '│';
			s[0] += new string(' ', width - midpos - 1);
		}else{
			s[0] = new string(' ', width);
		}
		
		s[1] = "";
		if(left){
			s[1] += new string('─', midpos);
		}else{
			s[1] += new string(' ', midpos);
		}
		
		if(left && right && corner){
			s[1] += '┼';
		}else if(left && right){
			s[1] += '┬';
		}else if(left && corner){
			s[1] += '┤';
		}else if(left){
			s[1] += '┐';
		}else if(right && corner){
			s[1] += '├';
		}else if(right){
			s[1] += '┌';
		}else if(corner){
			s[1] += '│';
		}else{
			s[1] += '?';
		}
		
		if(right){
			s[1] += new string('─', width - midpos - 1);
		}else{
			s[1] += new string(' ', width - midpos - 1);
		}
		
		s[2] = new string(' ', midpos);
		s[2] += '│';
		s[2] += new string(' ', width - midpos - 1);
		
		return s;
	}
	
	public override string ToString(){
		return "M" + (corner ? "^" : "");
	}
}

class Horizontal : DrawElement{
	public int corner = 0; //1 is up, 2 is down
	public bool left = false;
	public bool right = false;
	
	public override int getWidth(){
		return 1;
	}
	
	public override string[] getDrawString(int width){
		string[] s = new string[3];
		int midpos = (width-1)/2;
		
		if(corner == 1){
			s[0] = new string(' ', midpos);
			s[0] += '│';
			s[0] += new string(' ', width - midpos - 1);
		}else{
			s[0] = new string(' ', width);
		}
		
		s[1] = "";
		if(left){
			s[1] += new string('─', midpos);
		}else{
			s[1] += new string(' ', midpos);
		}
		
		if(left && right && corner == 1){
			s[1] += '┴';
		}else if(left && right && corner == 2){
			s[1] += '┬';
		}else if(left && right && corner == 0){
			s[1] += '─';
		}else if(left && corner == 1){
			s[1] += '┘';
		}else if(left && corner == 2){
			s[1] += '┐';
		}else if(left && corner == 0){
			s[1] += '─';
		}else if(right && corner == 1){
			s[1] += '└';
		}else if(right && corner == 2){
			s[1] += '┌';
		}else if(right && corner == 0){
			s[1] += '─';
		}else if(corner == 2 || corner == 1){
			s[1] += '│';
		}else{
			s[1] += '?';
		}
		
		if(right){
			s[1] += new string('─', width - midpos - 1);
		}else{
			s[1] += new string(' ', width - midpos - 1);
		}
		
		if(corner == 2){
			s[2] = new string(' ', midpos);
			s[2] += '│';
			s[2] += new string(' ', width - midpos - 1);
		}else{
			s[2] = new string(' ', width);
		}
		
		return s;
	}
	
	public override string ToString(){
		return "H" + (corner == 1 ? "^" : (corner == 2 ? "v" : ""));
	}
}

class Resistor : DrawElement{
	public string id;
	
	public Resistor(string d){
		id = d;
	}
	
	public override int getWidth(){
		return 2 + id.Length;
	}
	
	public override string[] getDrawString(int width){
		string[] s = new string[3];
		int midpos = (width-1)/2;
		
		s[0] = "╔";
		s[0] += new string('═', midpos - 1);
		s[0] += '╧';
		s[0] += new string('═', width - midpos - 2);
		s[0] += '╗';
		
		s[1] = "║";
		s[1] += id.PadRight(width-2);
		s[1] += '║';
		
		s[2] = "╚";
		s[2] += new string('═', midpos - 1);
		s[2] += '╤';
		s[2] += new string('═', width - midpos - 2);
		s[2] += '╝';
		
		return s;
	}
	
	public override string ToString(){
		return id;
	}
}

class DrawData{
	List<DrawElement[]> data;
	int cols;
	int currentRow;
	public DrawData(Circuit c){
		cols = c.getMaxColumns();
		
		data = new List<DrawElement[]>();
		
		int e = calcP(0, cols);
		
		addList(currentRow);
		data[currentRow][e] = new Branch();
		currentRow++;
		
		recursiveDraw(c.first, 0, cols);
		
		addList(currentRow);
		data[currentRow][e] = new Branch();
		currentRow++;
	}
	
	private void addList(int r){
		while(data.Count - 1 < r){
			data.Add(new DrawElement[cols]);
		}
	}
	
	private void recursiveDraw(CircuitNode cn, int a, int b){
		int e = calcP(a, b);
		while(true){
			if(cn is SeriesNode sn){
				addList(currentRow);
				data[currentRow][e] = new Resistor(sn.id);
				currentRow++;
			}else if(cn is ParallelNode pn){
				addList(currentRow);
				data[currentRow][e] = new Split();
				
				int[] v = pn.branchWidths();
				int sumv = 0;
				int f = pn.getNumberOfBranches();
				
				for(int i = 0; i < f; i++){
					sumv += v[i];
				}
				
				int sumg = b - sumv;
				
				int[] g = calcG(sumg, f - 1);
				
				int[] c = calcCArray(a, v, g, f);
				int[] p = calcPArray(c, v, f);
				
				int firstP = p[0];
				int lastP = p[f-1];
				
				for(int i = firstP; i <= lastP; i++){
					if(!(data[currentRow][i] is Split)){
						Horizontal h = new Horizontal();
						if(i != firstP){
							h.left = true;
						}
						if(i != lastP){
							h.right = true;
						}
						data[currentRow][i] = h;
					}
				}
				
				if(firstP < e && data[currentRow][e] is Split s1){
					s1.left = true;
				}
				
				if(lastP > e && data[currentRow][e] is Split s2){
					s2.right = true;
				}
				
				for(int i = 0; i < f; i++){
					if(data[currentRow][p[i]] is Horizontal h1){
						h1.corner = 2;
					}else if(data[currentRow][p[i]] is Split s3){
						s3.corner = true;
					}
				}
				currentRow++;
				int rowBefore = currentRow;
				
				int[] lengths = new int[f];
				int biggestLength = 0;
				
				for(int i = 0; i < f; i++){
					recursiveDraw(pn.branches[i], c[i], v[i]);
					lengths[i] = currentRow - rowBefore;
					currentRow = rowBefore;
					
					if(lengths[i] > biggestLength){
						biggestLength = lengths[i];
					}
				}
				
				for(int i = 0; i < f; i++){
					int addedLength = biggestLength - lengths[i];
					
					for(int j = 0; j < addedLength; j++){
						data[rowBefore + lengths[i] + j][p[i]] = new Branch();
					}
				}
				
				currentRow = rowBefore + biggestLength - 1;
				
				currentRow++;
				addList(currentRow);
				data[currentRow][e] = new Merge();
				
				for(int i = firstP; i <= lastP; i++){
					if(!(data[currentRow][i] is Merge)){
						Horizontal h = new Horizontal();
						if(i != firstP){
							h.left = true;
						}
						if(i != lastP){
							h.right = true;
						}
						data[currentRow][i] = h;
					}
				}
				
				for(int i = 0; i < f; i++){
					if(data[currentRow][p[i]] is Horizontal h2){
						h2.corner = 1;
					}else if(data[currentRow][p[i]] is Merge m1){
						m1.corner = true;
					}
				}
				
				if(firstP < e && data[currentRow][e] is Merge m2){
					m2.left = true;
				}
				
				if(lastP > e && data[currentRow][e] is Merge m3){
					m3.right = true;
				}
				
				currentRow++;
			}
			
			if(cn.next == null){
				return;
			}
			cn = cn.next;
		}
	}
	
	private static int calcP(int[] c, int[] v, int i){
		return c[i] + ((v[i]-1)/2);
	}
	
	private static int[] calcPArray(int[] c, int[] v, int f){
		int[] p = new int[f];
		for(int i = 0; i < f; i++){
			p[i] = c[i] + ((v[i]-1)/2);
		}
		return p;
	}
	
	private static int calcP(int c, int v){
		return c + ((v-1)/2);
	}
	
	private static int[] calcG(int sumg, int w){
		int[] g = new int[w];
		int c = sumg/w;
		for(int i = 0; i < w; i++){
			g[i] = c;
		}
		int d = sumg % w;
		for(int i = 0; i < d; i++){
			g[i]++;
		}
		return g;
	}
	
	private static int calcC(int a, int[] v, int[] g, int i){
		int sum = 0;
		for(int j = 0; j < i; j++){
			sum += v[i];
			sum += g[i];
		}
		return a + sum;
	}
	
	private static int[] calcCArray(int a, int[] v, int[] g, int f){
		int[] c = new int[f];
		for(int i = 0; i < f; i++){
			int sum = 0;
			for(int j = 0; j < i; j++){
				sum += v[j];
				sum += g[j];
			}
			c[i] = a + sum;
		}
		return c;
	}
	
	public string Draw(){
		string s = "";
		int[] maxWidths = new int[cols];
		for(int i = 0; i < cols; i++){
			int max = 0;
			for(int j = 0; j < data.Count; j++){
				if(data[j][i] == null){
					continue;
				}
				int m = data[j][i].getWidth();
				if(m > max){
					max = m;
				}
			}
			maxWidths[i] = max;
		}
		
		for(int j = 0; j < data.Count; j++){
			for(int k = 0; k < 3; k++){
				for(int i = 0; i < cols; i++){
					if(data[j][i] == null){
						s += new string(' ', maxWidths[i]);
						continue;
					}
					s += data[j][i].getDrawString(maxWidths[i])[k];
				}
				s += "\n";
			}
		}
		
		return s;
	}
	
	public override string ToString()
{
    string s = "";

    // Top border
    s += "┌";
    s += new string('─', cols * 5 - 1);
    s += "┐\n";

    for (int i = 0; i < data.Count; i++)
    {
        // Print each row's elements with vertical borders
        s += "│";
        for (int j = 0; j < cols; j++)
        {
            if (data[i][j] != null)
            {
                s += data[i][j].ToString().PadRight(4);
            }
            else
            {
                s += "    ";  // Empty cell
            }
            s += "│"; // Vertical border
        }
        s += "\n";

        // Horizontal border between rows
        s += "┼";
        s += new string('─', cols * 5 - 1);
        s += "┼\n";
    }

    return s;
}

#if debug
	private void log(object o, string variableName){
		Console.WriteLine("Name of variable: " + variableName);
		Console.WriteLine("Type: " + o.GetType());
		if(o is System.Collections.ICollection collection){
			Console.WriteLine("Length: " + collection.Count);
			string s = "Values: {";
			foreach(var v in collection){
				s += v.ToString();
				s += ", ";
			}
			if(s.Length > 2){
				s = s.Substring(0, s.Length - 2);
			}
			s += "}";
			Console.WriteLine(s);
		}else if(o is IEnumerable<object> enumerable)
		{
			Console.WriteLine("Length: " + enumerable.Count());
			string s = "Values: {";
			foreach(var v in enumerable){
				s += v.ToString;
				s += ", ";
			}
			if(s.Length > 2){
				s = s.Substring(0, s.Length - 2);
			}
			s += "}";
			Console.WriteLine(s);
		}else{
			Console.WriteLine("Value: " + o.ToString());
		}
		Console.WriteLine();
	}
#endif
}