import React, { Component } from 'react';
import { AccountInfo } from './AccountInfo';

export class Accounts extends Component {
    static displayName = Accounts.name;
  
    constructor (props) {
      super(props);
      this.state = 
      { 
        accounts: [], 
        loading: true,
        selectedAccount: '',
      };
    }

    componentDidMount() { 
        this.renderAccountsTable(this.state.accounts);
        setInterval(() => this.fetchAccounts(), 2000);
    }

    fetchAccounts()
    {
      //console.log("fetch accounts called");
      this.refreshAccountsData();      
    }

    refreshAccountsData = () =>
    {
      var data = fetch('api/Home/GetAccounts')
      .then(response => response.json())
      .then(data => {
          this.setState({ accounts: data, loading: false }, () => this.updateSelectedAccount(data));          
          return data;
      });
      return data;
    }

    updateSelectedAccount = (data) => { 
      if(data.length > 0 && this.state.selectedAccount.length > 0)
      {
        var account = data.filter(x => x.accountId === this.state.selectedAccount[0].accountId);
        this.setState({selectedAccount: account});
      }
    };

    enableAccountPanel = accountId => {
      this.props.showAccountsPanel(false);
      this.showAccountInfo(accountId);
    }

    showAccountInfo = accountId => {
      
      var account = this.state.accounts.filter(x => x.accountId === accountId);
      if(account.length > 0)
      {
        this.setState({selectedAccount: account});
      }
    }
  
    renderAccountsTable (accounts) {
      return (
        <div>
          <div id="accountList">
            <table className='table table-hover table-accounts-list table-striped'>
              <thead>
                <tr>
                  <th style={{width: '450px'}}>Account Id</th>
                  <th style={{width: '430px'}}>Name</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {accounts.map(account =>
                  <tr key={account.accountId}>
                    <td>{account.accountId}</td>
                    <td>{account.accountHolderName}</td>
                    <td><button onClick={() => this.enableAccountPanel(account.accountId)}>View account</button></td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>          
        </div>
      );
    }
  
    render () {
      //console.log(this.state.selectedAccount);
      let contents = this.state.loading
        ? <p><em>Loading all accounts...</em></p>
        : this.renderAccountsTable(this.state.accounts);
  
      return (
        <div>
          {contents}
          <div id="accountInfo" style={{display: 'none'}}>
            {/* <a onClick={() => this.props.showAccountsPanel(true)}>Back</a> */}
            <AccountInfo account={this.state.selectedAccount} showAccountsPanel={this.props.showAccountsPanel} refreshAccountsData={this.refreshAccountsData.bind(this)}/>
          </div>
        </div>
      );
    }
  }