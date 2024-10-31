using System;
using System.Text;

class Circuit{
	public CircuitNode first;
	public DrawData dd;
	
	public string name;
	public double voltage;
	public double intensity;
	public double power;
	
	private int idCounter = 1;
	private List<CircuitNode> all;
	
	public Circuit(string s)
    {
        string[] p = s.Split(":");
		
		if(p.Length < 3){
			throw new Exception("Not enough length!");;
		}
		
		name = p[0];
		
		int r = 1;
		int len = 0;
		while(true){
			if(p[r][0] == 'V'){
				voltage = double.Parse(p[r].Substring(2));
				len += p[r].Length;
				r++;
				break;
			}
			if(p[r][0] == 'I'){
				intensity = double.Parse(p[r].Substring(2));
				len += p[r].Length;
				r++;
				break;
			}
			break;
		}
		
        s = string.Join(":", p.Skip(r));  // Remove the initial "R:"
		int i = 0;
        first = ParseParallel(s, ref i);
		
		//Console.WriteLine("On parse: " + first.ToStringSimple());
		
		first = first.fixSingleParallel(null);
		first.fixNestedParallel();
		
		if(voltage > 0){
			intensity = voltage/getResistance();
		}else if(intensity > 0){
			voltage = intensity*getResistance();
		}else{
			throw new Exception("Not enough data");
		}
		power = intensity*voltage;
		
		first.propagateValues(intensity);
		
		assignLabels();
		registerNodes();
		
		dd = new DrawData(this);
    }
	
	public static Circuit Parse(string s){
		Circuit c = new Circuit(s);
        return c;
	}
	
	private static CircuitNode ParseSeries(string s, ref int index){
		CircuitNode firstNode = null;
		CircuitNode currentNode = null;
		CircuitNode previousNode = null;
		
		while(true){
			if(index > s.Length - 1){
				if(previousNode != null && currentNode != null){
					previousNode.next = currentNode;
				}
				if(firstNode == null && currentNode != null){
					firstNode = currentNode;
				}
				return firstNode;
			}
			switch(s[index]){
				case '.':
					if(previousNode != null && currentNode != null){
						previousNode.next = currentNode;
					}
					if(firstNode == null && currentNode != null){
						firstNode = currentNode;
					}
					previousNode = currentNode;
					currentNode = null;
					index++;
					break;
				case ',':
					if(previousNode != null && currentNode != null){
						previousNode.next = currentNode;
					}
					if(firstNode == null && currentNode != null){
						firstNode = currentNode;
					}
					index++;
					return firstNode;
				case ';':
					if(previousNode != null && currentNode != null){
						previousNode.next = currentNode;
					}
					if(firstNode == null && currentNode != null){
						firstNode = currentNode;
					}
					return firstNode;
				case ':':
					index++;
					if(previousNode != null && currentNode != null){
						previousNode.next = currentNode;
					}
					if(firstNode == null && currentNode != null){
						firstNode = currentNode;
					}
					previousNode = currentNode;
					currentNode = null;
					
					currentNode = ParseParallel(s, ref index);
					
					if(previousNode != null && currentNode != null){
						previousNode.next = currentNode;
					}
					if(firstNode == null && currentNode != null){
						firstNode = currentNode;
					}
					previousNode = currentNode;
					currentNode = null;
					break;
				default:
					double d = double.Parse(untilNextSeparator(s, ref index));
					currentNode = new SeriesNode(d);
					break;
			}
		}
	}
	
	private static CircuitNode ParseParallel(string s, ref int index){
		List<CircuitNode> branches = new List<CircuitNode>();
		
		ParallelNode pn = new ParallelNode(new CircuitNode[0]);
	
		while(true){
			// Parse the current branch which may consist of series nodes
			CircuitNode branchNode = ParseSeries(s, ref index);
			
			if (branchNode != null){	
				branches.Add(branchNode);
			}
			
			if(index > s.Length - 1 || s[index] == ';'){
				index++;
				pn.branches = branches.ToArray();
				CircuitNode cn = pn;
				return cn;
			}
		}
	}
	
	private static string untilNextSeparator(string s, ref int index){
		StringBuilder sb = new StringBuilder();
		int ind = index;
		while(true){
			if(ind > s.Length - 1){
				index = s.Length;
				return sb.ToString();
			}
			if(s[ind] == '.' || s[ind] == ',' || s[ind] == ';' || s[ind] == ':'){
				index = ind;
				return sb.ToString();
			}
			sb.Append(s[ind]);
			ind++;
		}
	}
	
	public string Draw(){
		string s = "";
		
		s += "Name: " + name + "\n";
		s += "\n";
		s += "Columns: " + getMaxColumns() + "\n";
		s += "Drawing:\n";
		s += "\n";
		s += dd.Draw();
		s += "\n";
		s += getDataTable();
		s += "\n\n";
		s += "Total circuit data:";
		s += "Total Resistace: " + getResistance() + " Ohms \n";
		s += "Total Voltage: " + voltage + " V \n";
		s += "Total Intensity: " + intensity + " A \n";
		s += "Total Power: " + power + " W \n";
		
		return s;
	}
	
	private string getDataTable(){
		string s = "";
		s += "####Data Table####\n";
		for(int i = 0; i < all.Count; i++){
			if(all[i] is SeriesNode sn){
				s += "Resistor ";
				s += all[i].id;
				s += ": Resistance: ";
				s += all[i].getResistance().ToString("0.##");
				s += " Ohms. Voltage: ";
				s += all[i].voltage.ToString("0.##");
				s += " V. Intensity: ";
				s += all[i].intensity.ToString("0.##");
				s += " A. Power: ";
				s += all[i].power.ToString("0.##");
				s += " W.";
			}else if(all[i] is ParallelNode pn){
				s += "Branch ";
				s += all[i].id;
				s += ": Branches: {";
				for(int j = 0; j < pn.getNumberOfBranches(); j++){
					s += pn.branches[j].id;
					s += ((j != pn.getNumberOfBranches() - 1) ? ", " : "");
				}
				s += "}. Total Resistance: ";
				s += all[i].getResistance().ToString("0.##");
				s += " Ohms. Total Voltage: ";
				s += all[i].voltage.ToString("0.##");
				s += " V. Total Intensity: ";
				s += all[i].intensity.ToString("0.##");
				s += " A. Total Power: ";
				s += all[i].power.ToString("0.##");
				s += " W.";
			}
			s += "\n";
		}
		return s;
	}
	
	public int getMaxColumns(){
		return first.maxColumns();
	}
	
	private int maxColumns(CircuitNode cn){
		int m = 1;
		while(true){
			
			if(cn is ParallelNode pn){
				int c = 0;
				for(int i = 0; i < pn.getNumberOfBranches(); i++){
					c += maxColumns(pn.branches[i]);
				}
				if(c > m){
					m = c;
				}
			}
			
			if(cn.next == null){
				break;
			}
			cn = cn.next;
		}
		return m;
	}
	
	public double getResistance(){
		CircuitNode cn = first;
		double b = 0d;
		while(cn != null){
			b += cn.getResistance();
			cn = cn.next;
		}
		return b;
	}
	
	private string getLabel(){
		string label = "";
		int counter = idCounter;
		while (counter > 0){
			counter--;
			label = (char)('A' + (counter % 26)) + label;
			counter /= 26;
		}
		idCounter++;
		return label;
	}
	
	private void assignLabels(){
		assignLabelsRecursive(first);
	}
	
	private void assignLabelsRecursive(CircuitNode cn){
		while(true){
			if(cn is SeriesNode sn){
				sn.id = getLabel();
			}else if(cn is ParallelNode pn){
				pn.id = getLabel();
				for(int i = 0; i < pn.getNumberOfBranches(); i++){
					assignLabelsRecursive(pn.branches[i]);
				}
			}
			
			if(cn.next == null){
				break;
			}
			cn = cn.next;
		}
	}
	
	private void registerNodes(){
		all = new List<CircuitNode>();
		registerNodesRecursive(first);
	}
	
	private void registerNodesRecursive(CircuitNode cn){
		while(true){
			if(cn is SeriesNode sn){
				all.Add(cn);
			}else if(cn is ParallelNode pn){
				all.Add(cn);
				for(int i = 0; i < pn.getNumberOfBranches(); i++){
					registerNodesRecursive(pn.branches[i]);
				}
			}
			
			if(cn.next == null){
				break;
			}
			cn = cn.next;
		}
	}
	
	public static void help(){
		Console.WriteLine("This program aims to help with solving simple electrical circuits. For example:");
		Circuit c = new Circuit("Circuit 1:V.12:300.150");
		Console.WriteLine(c.dd.Draw());
		Console.WriteLine("This circuit would be written: \"Circuit 1:V.12:300.150\"");
		Console.WriteLine("Resistor A is 300 ohms and resistor B is 150, and its connected to 12 volts");
		Console.WriteLine();
		Console.WriteLine("More examples:");
		c = new Circuit("Circuit 2:V.12:100,100");
		Console.WriteLine(c.dd.Draw());
		Console.WriteLine("This circuit would be written: \"Circuit 2:V.12:100,100\"");
		c = new Circuit("Circuit 2:V.12:100.100,100.100.100");
		Console.WriteLine(c.dd.Draw());
		Console.WriteLine("This circuit would be written: \"Circuit 2:V.12:100.100,100.100.100\"");
		c = new Circuit("Circuit 2:V.12:100.100:100,100;100");
		Console.WriteLine(c.dd.Draw());
		Console.WriteLine("This circuit would be written: \"Circuit 2:V.12:100.100:100,100;100\"");
		Console.WriteLine();
		Console.WriteLine("We can see, \".\" is used for series, \":\" for opening parallel branches, \",\" for passing to the next branch of the parallel, and \";\" for finishing parallels");
		Console.WriteLine();
	}
	
	static void Main()
    {
        while(true){
			try{
				Console.WriteLine("Enter the circuit: ");
				string s = Console.ReadLine();
				
				switch(s){
					case "help":
					help();
					continue;
					
					case "exit":
					Environment.Exit(0);
					continue;
				}
				
				Circuit c = new Circuit(s);
				if(c == null){
					continue;
				}
				
				Console.WriteLine(c.Draw());
			}catch(Exception e){
				Console.WriteLine("Exception occured!: " + e.ToString());
			}
		}
    }
}