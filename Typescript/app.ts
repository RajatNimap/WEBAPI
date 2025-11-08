// let a:number=309
// a=20
// console.log(a)
function hello(num:number):number{
    return num
}

// console.log(hello(30));
export {}
const happy= (num:number):string=>{

    return ` great you have ${num} Ruppes`

}
const originalArray=["rajat","pandit","niraj","Suraj","Akshay"];
// console.log(happy(10))

const Hello=originalArray.map((originalArray):string=>{
        //console.log(originalArray,"you are the healthy personn"  );
        return originalArray
})
//console.log(Hello);

//Void type
function voitype():void{
    console.log("This is void type ");
}
voitype();

//never type for throwing exception
function nevertype():never{
        throw new Error("Not implemented");
}
nevertype();