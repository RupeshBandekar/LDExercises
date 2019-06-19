import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Welcome!</h1>
        <p>This is Account Balance Client application using <code>React.js</code>.</p>        
        <p>Please click on <code>Accounts</code> menu to move ahead!</p>
      </div>
    );
  }
}
