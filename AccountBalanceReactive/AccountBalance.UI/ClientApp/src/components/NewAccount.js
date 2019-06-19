import React, { Component } from 'react';

export class NewAccount extends Component {
    static displayName = NewAccount.name;
  
    constructor (props) {
      super(props);
      this.state = {accountHolderName: '', message: '', loading: true };
    }
  
    createNewAccount = (event) => {
        event.preventDefault();
        this.postData('api/Home/CreateAccount', {accountHolderName: this.state.accountHolderName})
            .then(data => this.setState({message: data})) //JSON-string from `response.json()` call
            .catch(error => console.error(error));
        this.setState({accountHolderName: ''});
    }

    postData = (url = '', data = {}) => {
        // Default options are marked with *
        return fetch(url, {
                method: 'POST', // *GET, POST, PUT, DELETE, etc.
                mode: 'cors', // no-cors, cors, *same-origin
                cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
                credentials: 'same-origin', // include, *same-origin, omit                
                headers: {
                    'Content-Type': 'application/json',//'application/json',
                    // 'Content-Type': 'application/x-www-form-urlencoded',
                },
                redirect: 'follow', // manual, *follow, error
                referrer: 'no-referrer', // no-referrer, *client
                body: JSON.stringify(data), // body data type must match "Content-Type" header
            })
            .then(response => response.json()); //response.json() parses JSON response into native Javascript objects 
    }
  
    render () {
      //console.log("New Account rendered");
      return (
        <form onSubmit={this.createNewAccount}>
          <div>
            <table className='table table-create-new'>  
              <thead>
                <tr>
                  <th colSpan="2">
                    Create New Account
                  </th>
                </tr>
              </thead>       
              <tbody>
                <tr>
                  <td style={{width: '250px'}}><label>Account Holder Name:</label></td>
                  <td><input id="txtAccountHolderName" type="text" value={this.state.accountHolderName} 
                  onChange={event => {this.setState({accountHolderName: event.target.value, message: ''});}} required
                  style={{width: '300px'}}/></td>
                </tr>
                <tr>
                  <td></td>
                  <td><button id="btnCreate">Create Account</button></td>
                </tr>
                <tr>
                  <td></td>
                  <td><label id="lblMessage">{this.state.message}</label></td>
                </tr>
            </tbody>
          </table>
          </div>
        </form>
      );
    }
  }