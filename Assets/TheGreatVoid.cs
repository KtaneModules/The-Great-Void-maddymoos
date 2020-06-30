using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using KModkit;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class TheGreatVoid : MonoBehaviour {
	
	public Transform THEVOID;
	public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Bomb;
    public TextMesh[] Text;
    public Light why;
    public KMSelectable Button;
	public Renderer[] Segments;
	public Color[] Colors;
	private string[] CNames = {"Red","Green","Blue","Magenta","Yellow","Cyan","White"};
	private int[] Displays = new int[6];
	private int[] ColorNums = new int[6];
	private static string[] Map = {
		"-110-001-",
		"010111002",
		"100211221",
		"221122110",
		"-0102100-",
		"011120201",
		"101001112",
		"200211110",
		"-112-012-"
	};
	private int Direction;
	private string[] SideTernary = new string[6];
	private string Answer;
	private string Garbage;
	private bool Solved = false;
	private bool Going=false;
	static private int _moduleIdCounter = 1;
	private int _moduleId;
	void Awake () {
        _moduleId = _moduleIdCounter++;
        Button.OnInteract += delegate () {HandlePress();return false;};
        Button.OnInteractEnded += delegate () {HandlePress();return;};
	}	
	void Submit(){
		Debug.Log(Garbage);
		bool yes = true;
		yes=true;
		if(Answer.Join("")!=Garbage.Join(""))
			yes=false;
		if(yes){
			Audio.PlaySoundAtTransform("Solve", THEVOID.transform);
			Module.HandlePass();
			Solved = true;
			for(int i=0;i<6;i++){
				Text[i].text="";
				Segments[i].material.SetColor("_Color",Colors[7]);
			}
			Segments[6].material.SetColor("_Color",Colors[1]);
			StartCoroutine(Colorfade());
			Debug.LogFormat("[The Great Void #{0}]: Correct! Module solved.",_moduleId);
		}
		else{
			Module.HandleStrike();
			Segments[6].material.SetColor("_Color",Colors[7]);
			char[] Chars=Garbage.ToArray();
			int State = 0;
			for(int i=0;i<Chars.Length;i++){
			if(Chars[i]=='i'){
				if(State==1){
					Chars[i]=']';
					State=0;
				}
				else if(State==0){
					Chars[i]='[';
					State=1;
				}
			}
		}
		Garbage = Chars.Join("");
		Chars = Answer.ToArray();
		State = 0;
		for(int i=0;i<Chars.Length;i++){
			if(Chars[i]=='i'){
				if(State==1){
					Chars[i]=']';
					State=0;
				}
				else if(State==0){
					Chars[i]='[';
					State=1;
				}
			}
		}
		string TempAnswer = Chars.Join("");
		Debug.LogFormat("[The Great Void #{0}]: You submitted {1} when I wanted {2}. Strike.",_moduleId,Garbage,TempAnswer);
		Garbage="";
		}
	}
	void HandlePress(){
		if(Solved){
			return;
		}
		if(!Going){
			Going=true;
			StartCoroutine(Timer());
		}
		Garbage+="i";
	}
	// Use this for initialization
	void Start () {
		for(int i=0;i<6;i++){
			Displays[i] = Rnd.Range(0,7);
			ColorNums[i] = Rnd.Range(0,7);
			Text[i].text = Displays[i].ToString();
			Segments[i].material.SetColor("_Color", Colors[ColorNums[i]]);
		}
		Debug.LogFormat("[The Great Void #{0}]: The numbers in face order are {1}", _moduleId, Displays.Join(""));
		Debug.LogFormat("[The Great Void #{0}]: The colors in face order are {1}, {2}, {3}, {4}, {5}, {6}", _moduleId, CNames[ColorNums[0]],CNames[ColorNums[1]],CNames[ColorNums[2]],CNames[ColorNums[3]],CNames[ColorNums[4]],CNames[ColorNums[5]]);
		StartCoroutine(RotateSphere());
		Calculate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void Calculate (){
		int[] Xdir = { 0, 1, 1, 1, 0,-1,-1,-1};
		int[] Ydir = {-1,-1, 0, 1, 1, 1, 0,-1};
		for(int i=0;i<6;i++){
			int x = ColorNums[i]+1;
			int y = Displays[i]+2;
			int Dir = 0;
			string stuff = "";
			int sum = 0;
			for(int a=0;a<3;a++){
				sum = 0;
				x = (x+Xdir[Dir]);
				y = (y+Ydir[Dir]);
				if(x==9||y==9||x==-1||y==-1){
					if(x==9||x==-1) Dir=(8-Dir)%8;
					if(y==9||y==-1) Dir=(12-Dir)%8;
					if(x==-1)x=0;
					if(y==-1)y=0;
					if(x==9)x=8;
					if(y==9)y=8;
				}
				if(Map[y][x] == '-'){
					if(y==8||y==0) y=8-y;
					if(x==8||x==0) x=8-x;
					x = (x+Xdir[Dir]);
					y = (y+Ydir[Dir]);
					if(x==9||y==9||x==-1||y==-1){
					if(x==9||x==-1) Dir=(8-Dir)%8;
					if(y==9||y==-1) Dir=(12-Dir)%8;
					if(x==-1)x=0;
					if(y==-1)y=0;
					if(x==9)x=8;
					if(y==9)y=8;
				}
				}
				sum+=int.Parse(Map[y][x].ToString());
				if(a!=0)
				Dir=(Dir+1+i)%8;
				for(int b=0;b<a;b++){
					x = (x+Xdir[Dir]);
					y = (y+Ydir[Dir]);
					if(x==9||y==9||x==-1||y==-1){
						if(x==9||x==-1) Dir=(8-Dir)%8;
						if(y==9||y==-1) Dir=(12-Dir)%8;
						if(x==-1)x=0;
						if(y==-1)y=0;
						if(x==9)x=8;
						if(y==9)y=8;
					}
					if(Map[y][x] == '-'){
						if(y==8||y==0) y=8-y;
						if(x==8||x==0) x=8-x;
						x = (x+Xdir[Dir]);
						y = (y+Ydir[Dir]);
						if(x==9||y==9||x==-1||y==-1){
					if(x==9||x==-1) Dir=(8-Dir)%8;
					if(y==9||y==-1) Dir=(12-Dir)%8;
					if(x==-1)x=0;
					if(y==-1)y=0;
					if(x==9)x=8;
					if(y==9)y=8;
				}
				}
				sum+=int.Parse(Map[y][x].ToString());
			}
			sum%=3;
			stuff+=sum;
			}
			SideTernary[i]=stuff;
		}
		Debug.LogFormat("[The Great Void #{0}]: The digits in face order are: {1}", _moduleId, SideTernary.Join(", "));
		string Holder;
		if(Displays[0] > Displays[3]){
			Holder=SideTernary[1];
			SideTernary[1]=SideTernary[2];
			SideTernary[2]=Holder;
		}
		if(Displays[4] > Displays[1]){
			Holder=SideTernary[3];
			SideTernary[3]=SideTernary[5];
			SideTernary[5]=Holder;
		}
		if(Displays[2] > Displays[5]){
			Holder=SideTernary[4];
			SideTernary[4]=SideTernary[0];
			SideTernary[0]=Holder;
		}
		if(ColorNums[2]==0){
			Holder=SideTernary[0];
			SideTernary[0]=SideTernary[1];
			SideTernary[1]=SideTernary[2];
			SideTernary[2]=SideTernary[3];
			SideTernary[3]=SideTernary[4];
			SideTernary[4]=SideTernary[5];
			SideTernary[5]=Holder;
		}
		if(ColorNums[5]==1){
			SideTernary.Reverse();
		}
		if(ColorNums[0]==2){
			Holder=SideTernary[0];
			SideTernary[0]=SideTernary[1];
			SideTernary[1]=Holder;
			Holder=SideTernary[2];
			SideTernary[2]=SideTernary[3];
			SideTernary[3]=Holder;
			Holder=SideTernary[4];
			SideTernary[4]=SideTernary[5];
			SideTernary[5]=Holder;
		}
		Debug.LogFormat("[The Great Void #{0}]: The reordered digits are: {1}", _moduleId, SideTernary.Join(", "));
		Garbage=SideTernary.Join("");
		Debug.Log(Garbage);
		char[] Chars = new char[18];
		Chars = Garbage.ToArray();
		Chars[0] = '1';
		for(int i=0;i<Chars.Length;i++){
			if(Chars[i]=='2'){
				if(Chars.Count((char x) => x == 'i')%2==1)
					Chars[i]='i';
				else if(i!=0&&Chars[i-1]=='p'){
					Chars[i]='i';
				}
				else Chars[i]='p';
			}
			else if(Chars[i]=='1')
				Chars[i]='i';
			else{
				if(i!=0&&Chars[i-1]!='p'){
					Chars[i]='p';
				}
				else
					Chars[i]='i';
			}
		}
		Garbage = Chars.Join("");
		if(Garbage.Count((char x) => x == 'i')%2==1) Garbage+='i';
		Answer = Garbage;
		Chars = Garbage.ToArray();
		int State=0;
		for(int i=0;i<Chars.Length;i++){
			if(Chars[i]=='i'){
				if(State==1){
					Chars[i]=']';
					State=0;
				}
				else if(State==0){
					Chars[i]='[';
					State=1;
				}
			}
		}
		Garbage = Chars.Join("");
		Debug.LogFormat("[The Great Void #{0}]: The input sequence is {1}", _moduleId, Garbage);
		Garbage = "";
	}
	private IEnumerator RotateSphere() //Coroutine
	{
	bool rotateSphere = true;
	float x,y,z;
	float ex,wy,ze;
	float X,Y,Z;
	float EX,WY,ZE;
	ex = Rnd.Range(.005f,.01f);
	wy = Rnd.Range(.005f,.01f);
	ze = Rnd.Range(.005f,.01f);
	X = Rnd.Range(0,90);
	Y = Rnd.Range(0,90);
	Z = Rnd.Range(0,90);
	EX = 0;
	WY = 0;
	ZE = 0;
	x = 0;
	y = 0;
	z = 0;
	while(rotateSphere){

	THEVOID.localEulerAngles = new Vector3(Mathf.LerpAngle(EX,  X, x * x * (3.0f - 2.0f * x)),Mathf.LerpAngle(THEVOID.localEulerAngles.y,  THEVOID.localEulerAngles.y, 0),Mathf.LerpAngle(THEVOID.localEulerAngles.z,  THEVOID.localEulerAngles.z, 0));
	THEVOID.localEulerAngles = new Vector3(Mathf.LerpAngle(THEVOID.localEulerAngles.x,  THEVOID.localEulerAngles.x, 0),Mathf.LerpAngle(WY,  Y, y * y * (3.0f - 2.0f * y)),Mathf.LerpAngle(THEVOID.localEulerAngles.z,  THEVOID.localEulerAngles.z, 0));
	THEVOID.localEulerAngles = new Vector3(Mathf.LerpAngle(THEVOID.localEulerAngles.x,  THEVOID.localEulerAngles.x, 0),Mathf.LerpAngle(THEVOID.localEulerAngles.y,THEVOID.localEulerAngles.y,0),Mathf.LerpAngle(ZE,  Z, z *  z* (3.0f - 2.0f * z)));
	x+=ex;
	y+=wy;
	z+=ze;
	if(x>=1){
		EX = X;
		ex = Rnd.Range(.005f,.01f);
		X = (X+Rnd.Range(0,90))%360;
		x = 0;
	}if(y>=1){
		WY = Y;
		wy = Rnd.Range(.005f,.01f);
		Y = (Y+Rnd.Range(0,90))%360;
		y = 0;
	}if(z>=1){
		ZE = Z;
		ze = Rnd.Range(.005f,.01f);
		Z = (Z+Rnd.Range(0,90))%360;
		z = 0;
	}
	yield return new WaitForSeconds(.01f);
	}
	}
	private IEnumerator Timer(){
		int StrikeInt=0;
		while(Going){
			Segments[6].material.SetColor("_Color",Colors[7]);
			yield return new WaitForSeconds(.9f);
			Garbage+="p";
			if(Garbage.Length>=2){
			if(Garbage[Garbage.Length-1]=='p'&&Garbage[Garbage.Length-2]=='p'){
				char[] Chars = Garbage.ToArray();
				Chars[Chars.Length-1] = '|';
				if(Answer[Answer.Length-1]!='p')
				Chars[Chars.Length-2] = '|';
				Garbage = Chars.Join("");
				Garbage = Garbage.Replace("|","");
				Submit();
				Going=false;
				yield break;
			}
			else if(StrikeInt==1)
				Submit();
			}
			else StrikeInt=1;
			StartCoroutine(Colorfade2());
			yield return new WaitForSeconds(.1f);
		}
	}
	public IEnumerator Colorfade() {
		byte g=255;
		while(g>0){
			g-=5;
			for(int i=0;i<7;i++)
				Segments[i].material.SetColor("_Color",new Color32(0,g,0,255));
			yield return new WaitForSeconds(0.02f);
		}
		Segments[6].transform.localScale = new Vector3(0,0,0);
		StopAllCoroutines();
	}
	public IEnumerator Colorfade2() {
		Audio.PlaySoundAtTransform("Litning", THEVOID.transform);
		byte r=255;
		byte g=255;
		byte b=255;
		while(g>32){
			r-=32;
			g-=32;
			b-=32;
			Segments[6].material.SetColor("_Color",new Color32(r,g,b,255));
			why.color = new Color32(r,g,b,255);
			yield return new WaitForSeconds(0.02f);
		}
		Segments[6].material.SetColor("_Color",new Color32(0,0,0,255));
		why.color = new Color32(0,0,0,255);
	}
	#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} []p[p][][p]p (inputs the sequence)";
	#pragma warning restore 414
	IEnumerator ProcessTwitchCommand(string command)
    {
        Match m;
        if ((m = Regex.Match(command, @"^\s*([\[\]p]+)+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {    
            yield return null;
            var input = m.Groups[1].Value;        
            for(int i=0;i<m.Length;i++)
            {
                switch (m.Groups[1].Value[i])
                {
                    case '[': Button.OnInteract(); break;
                    case ']': Button.OnInteractEnded(); break;
                    case 'p': yield return new WaitForSeconds(1.05f); break;
                    case ' ': break;
                    default: break;
                }
            }
        }
        else
        {
            yield return "sendtochaterror Incorrect Syntax. Must use characters [, ], and p.";
            yield break;
        }
    }
	IEnumerator TwitchHandleForcedSolve()
    {
		bool held = false;
		for(int i=0;i<Answer.Length;i++){
			switch (Answer[i]){
			   case 'i': if(held){Button.OnInteractEnded(); held=false;}else{Button.OnInteract(); held=true;} break;
			   case 'p': yield return new WaitForSeconds(1.05f); break;
		   }
		}
    }
}
