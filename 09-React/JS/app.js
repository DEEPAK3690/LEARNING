function add(a, b) {
    return a + b;
}

const mul = (a, b) => a * b; // Arrow function for multiplication

var a = 5;//func scoped     
let b = 10;//block scoped
let result = add(a, b);

import { car } from './info.js';
import { Person } from './info.js';

const person = new Person();
person.printMyName();

console.log(car.getCarInfo());