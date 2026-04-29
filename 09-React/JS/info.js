export const car = {
  type: "Fiat",
  model: "500",
  color: "white",
  getCarInfo: function() {
    return "The car is a " + this.color + " " + this.type + " " + this.model + ".";
  }
};

export class Person{
  constructor(){
    this.name = 'John';
    this.age = 30;
  }
  
  printMyName(){
    console.log(this.name);
    console.log(this.age);
  }
}


