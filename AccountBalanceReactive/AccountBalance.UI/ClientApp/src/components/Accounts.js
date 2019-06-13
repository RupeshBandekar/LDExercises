import React, { Component } from 'react';

export class Accounts extends Component {
    static displayName = Accounts.name;
  
    constructor (props) {
      super(props);
      this.state = { accounts: [], loading: true };
    }

    componentDidMount() {
        Accounts.renderAccountsTable(this.state.accounts);
        var intervalId = setInterval(() => this.fetchAccounts(), 3000);
    }

    fetchAccounts()
    {
        console.log("data fetched");
        fetch('api/Home/GetAccounts')
        .then(response => response.json())
        .then(data => {
            this.setState({ accounts: data, loading: false });
        });
    }
  
    static renderAccountsTable (accounts) {
      return (
        <div>
          <table className='table table-striped'>
            <thead>
              <tr>
                <th>Account Id</th>
                <th>Name</th>
              </tr>
            </thead>
            <tbody>
              {accounts.map(account =>
                <tr key={account.accountId}>
                  <td><a>{account.accountId}</a></td>
                  <td><a>{account.accountHolderName}</a></td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      );
    }
  
    render () {
      let contents = this.state.loading
        ? <p><em>Loading...</em></p>
        : Accounts.renderAccountsTable(this.state.accounts);
  
      return (
        <div>
          {contents}
        </div>
      );
    }
  }