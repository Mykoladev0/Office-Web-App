import React, {Component} from 'react';
import {ReactDOM} from 'react-dom';

export default class Counter extends Component {
  constructor() {
    super();
    this.state = { currentCount: 0 };
   }
  render() {
   return (
   <div>
       <h1>Counter</h1>
      <p>This is a simple example of a React component.</p>
      <p>Current count: <strong>{this.state.currentCount}</strong></p>
      <button onClick={() => { this.incrementCounter()}}>Increment</button>
      </div>
      );
  }

 incrementCounter() {
   this.setState({
     currentCount: this.state.currentCount + 1
         });
    }
}