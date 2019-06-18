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
        setInterval(() => this.fetchAccounts(), 3000);
    }

    fetchAccounts()
    {
      console.log("fetch accounts called");
      this.refreshAccountsData();      
    }

    refreshAccountsData()
    {
      fetch('api/Home/GetAccounts')
      .then(response => response.json())
      .then(data => {
          this.setState({ accounts: data, loading: false }, () => this.updateSelectedAccount(data));
      });
    }

    updateSelectedAccount = (data) => {
      if(data.length > 0 && this.state.selectedAccount.length > 0)
      {
        var account = data.filter(x => x.accountId == this.state.selectedAccount[0].accountId);
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
      console.log("show account info called");
      //this.props.captureAccountId(accountId);

    }
  
    renderAccountsTable (accounts) {
      return (
        <div>
          <div id="accountList">
            <table className='table table-striped'>
              <thead>
                <tr>
                  <th>Account Id</th>
                  <th>Name</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {accounts.map(account =>
                  <tr key={account.accountId}>
                    <td><a onClick={() => this.enableAccountPanel(account.accountId)}>{account.accountId}</a></td>
                    <td><a>{account.accountHolderName}</a></td>
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
      console.log(this.state.selectedAccount);
      let contents = this.state.loading
        ? <p><em>Loading...</em></p>
        : this.renderAccountsTable(this.state.accounts);
  
      return (
        <div>
          {contents}
          <div id="accountInfo" style={{display: 'none'}}>
            {/* <a onClick={() => this.props.showAccountsPanel(true)}>Back</a> */}
            <AccountInfo account={this.state.selectedAccount} showAccountsPanel={this.props.showAccountsPanel}/>
          </div>
        </div>
      );
    }
  }