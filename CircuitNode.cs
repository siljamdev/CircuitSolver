using System;

abstract class CircuitNode{
	
	private static int idCounter = 1;
	
	public CircuitNode next;
	public string id;
	
	public double voltage;
	public double intensity;
	public double power;
	
	public abstract double getResistance();
	
	public abstract override string ToString();
	
	public abstract string ToStringSimple();
	
	public abstract CircuitNode fixSingleParallel(CircuitNode prev);
	
	public abstract void fixNestedParallel();
	
	public abstract void propagateValues(double intens);
	
	public abstract int maxColumns();
}

class SeriesNode : CircuitNode{
	public double resistance;
	
	public SeriesNode(double r){
		resistance = r;
	}
	
	public override double getResistance(){
		return resistance;
	}
	
	public override CircuitNode fixSingleParallel(CircuitNode prev){
		if(next == null){
			return this;
		}
		next = next.fixSingleParallel(this);
		return this;
	}
	
	public override void fixNestedParallel(){
		if(next == null){
			return;
		}
		next.fixNestedParallel();
	}
	
	public override void propagateValues(double intens){
		intensity = intens;
		voltage = intens * resistance;
		power = intens * intens * resistance;
		if(next == null){
			return;
		}
		next.propagateValues(intens);
	}
	
	public override int maxColumns(){
		int m = 1;
		
		if(next == null){
			return m;
		}
		
		int n = next.maxColumns();
		if(n > m){
			m = n;
		}
		
		return m;
	}
	
	public override string ToString(){
		return "(R " + resistance.ToString("0.##") + ",V " + voltage.ToString("0.##") + ",I " + intensity.ToString("0.##") + ")" + next;
	}
	
	public override string ToStringSimple(){
		return "(R " + resistance.ToString("0.##") + ")" + ((next != null) ? next.ToStringSimple() : "");
	}
}

class ParallelNode : CircuitNode{
	public CircuitNode[] branches;
	public double[] branchIntensities;
	
	public ParallelNode(CircuitNode[] b){
		branches = b;
	}
	
	public int getNumberOfBranches(){
		return branches.Length;
	}
	
	public double getResistanceOfBranch(int i){
		double b = 0d;
		CircuitNode cn = branches[i];
		while(cn != null){
			b += cn.getResistance();
			cn = cn.next;
		}
		return b;
	}
	
	public override double getResistance(){
		double t = 0d;
		for(int i = 0; i < branches.Length; i++){
			double b = getResistanceOfBranch(i);
			t += 1.0d/b;
		}
		return 1.0d/t;
	}
	
	public override CircuitNode fixSingleParallel(CircuitNode prev){
		CircuitNode ret = this;
		if(getNumberOfBranches() == 1){
			branches[0] = branches[0].fixSingleParallel(null);
			
			ret = branches[0];

			if(prev != null){
				prev.next = branches[0];
			}
			
			CircuitNode p = branches[0];
			while(true){
				if(p.next == null){
					p.next = next;
					break;
				}
				p = p.next;
			}
		}else{
			for(int i = 0; i < getNumberOfBranches(); i++){
				branches[i] = branches[i].fixSingleParallel(null);
			}
		}
		
		if(ret.next == null){
			return ret;
		}
		
		ret.next = ret.next.fixSingleParallel(ret);
		return ret;
	}
	
	public override void fixNestedParallel(){
		List<CircuitNode> newBranches = new List<CircuitNode>();
		
		for(int i = 0; i < getNumberOfBranches(); i++){
			branches[i].fixNestedParallel();

			if(branches[i] is ParallelNode bn && bn.next == null){
				newBranches.AddRange(bn.branches);
			}else{
				newBranches.Add(branches[i]);
			}
			
		}
		branches = newBranches.ToArray();
		
		if(next == null){
			return;
		}
		next.fixNestedParallel();
		return;
	}
	
	public override void propagateValues(double intens){
		intensity = intens;
		voltage = intens * getResistance();
		power = intens * intens * getResistance();
		
		for(int i = 0; i < branches.Length; i++){
			double resb = getResistanceOfBranch(i);
			double insb = voltage/resb;
			
			branches[i].propagateValues(insb);
		}
		if(next == null){
			return;
		}
		next.propagateValues(intens);
	}
	
	public override int maxColumns(){
		int m = 1;
		
		int c = 0;
		for(int i = 0; i < getNumberOfBranches(); i++){
			c += branches[i].maxColumns();
		}
		if(c > m){
			m = c;
		}
		
		if(next == null){
			return m;
		}
		
		int n = next.maxColumns();
		if(n > m){
			m = n;
		}
		
		return m;
	}
	
	public int[] branchWidths(){
		int[] v = new int[getNumberOfBranches()];
		for(int i = 0; i < getNumberOfBranches(); i++){
			v[i] = branches[i].maxColumns();
		}
		return v;
	}
	
	public override string ToString(){
		string s = "{";
		for(int i = 0; i < getNumberOfBranches(); i++){
			s += branches[i] + (i == (getNumberOfBranches() - 1) ? "" : ",");
		}
		s += "}";
		s += next;
		return s;
	}
	
	public override string ToStringSimple(){
		string s = "{";
		for(int i = 0; i < getNumberOfBranches(); i++){
			s += branches[i].ToStringSimple() + (i == (getNumberOfBranches() - 1) ? "" : ",");
		}
		s += "}";
		s += ((next != null) ? next.ToStringSimple() : "");
		return s;
	}
}