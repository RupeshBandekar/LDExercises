import React, { Component } from 'react';

export class NewAccount extends Component {
    static displayName = NewAccount.name;
  
    constructor (props) {
      super(props);
      this.state = {accountHolderName: '', message: '', loading: true };
  
    //   fetch('api/Accounts/GetAccounts')
    //     .then(response => response.json())
    //     .then(data => {
    //       this.setState({ accounts: data, loading: false });
    //     });
    }
  
    static createNewAccount () {
      console.log(this.state.accountHolderName);
    }
  
    render () {   
      return (
        <div>
          <table className='table table-striped'>         
            <tbody>            
              <tr>
                <td>Account holder name:</td>
                <td><input id="txtAccountHolderName" type="text" value={this.state.accountHolderName} onChange={event => {this.setState({accountHolderName: event.target.value});}} /></td>
              </tr>
              <tr>
                <td></td>
                <td><button id="btnCreate" onClick={this.createNewAccount}>Create</button></td>
              </tr>
              <tr>
                <td></td>
                <td>{/* <lable id="lblMessage" value={this.state.message}></lable> */}</td>
              </tr>
          </tbody>
        </table>
        </div>
      );
    }
  }