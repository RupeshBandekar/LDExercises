import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import {NewAccount} from './NewAccount';
import {Accounts} from './Accounts';
import { AccountInfo } from './AccountInfo';

export class AccountContainer extends Component {
    state= {
        loadAccountList: true,
    };

    // componentWillMount(){
    //     this.accountId = '';
    
    // }

    // captureAccountId(accountId){
    //     this.accountId = accountId;
    //     console.log(this.accountId);
    // }
    
    showAccountsPanel = (showAccountList) => {
        //console.log("toggle");        
        if (showAccountList) {
            document.getElementById("accountList").style.display = "block";
            document.getElementById("newAccount").style.display = "block";
            document.getElementById("accountInfo").style.display = "none";
        } else {
            document.getElementById("newAccount").style.display = "none";
            document.getElementById("accountList").style.display = "none";
            document.getElementById("accountInfo").style.display = "block";
        }
        this.setState({loadAccountList: showAccountList});    
        //this.forceUpdate();    
    }

    render(){        
        //console.log("Container rendered");
        return(
            <div>
                <h1>Account Balance</h1>
                <div id="newAccount">
                <NewAccount />
                </div>
                <Accounts loadAccounts={this.state.loadAccountList} showAccountsPanel={this.showAccountsPanel} />
            </div>
        );
    }
}