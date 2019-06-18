import React, { Component } from 'react';
import {AccountDashboard} from './AccountDashboard';
import {OverdraftLimit} from './OverdraftLimit';
import {DailyWireTransferLimit} from './DailyWireTransferLimit';
import { DepositCheque } from './DepositCheque';
import { DepositCash } from './DepositCash';
import { WithdrawCash } from './WithdrawCash';
import { WireTransfer } from './WireTransfer';

export class AccountInfo extends Component {
    
    state = {
        accountId: '',
    }

    togglePanels = (panelName) => {
        //console.log(panelName);
        var panels = document.getElementsByClassName("actionPanel");
        for(var i = 0; i < panels.length; i++)
        {
            panels[i].style.display = "none";
        }
        if(panelName != "")
        {
            var panel = document.getElementById(panelName);
            panel.style.display = "block";
        }
        //this.forceUpdate();
    };

    handleBackClick = () => {
        this.props.showAccountsPanel(true);
        this.togglePanels("initialPanel");
    }
    
    render(){
        let component;
        if(this.props.account.length === 0)
        {
            component = null;
        }
        else {
            // console.log("above dashboard");
            // console.log(this.props.account);
            component = 
            <div>
                <AccountDashboard  account={this.props.account}/>
                <div>
                    <ul style={{listStyleType: 'none'}}>
                        <li><a onClick={() => this.togglePanels("overdraftLimit")}>Overdraft Limit</a></li>
                        <li><a onClick={() => this.togglePanels("wiretransferLimit")}>Daily Wiretransfer Limit</a></li>
                        <li><a onClick={() => this.togglePanels("depositCheque")}>Deposit Cheque</a></li>
                        <li><a onClick={() => this.togglePanels("depositCash")}>Deposite Cash</a></li>
                        <li><a onClick={() => this.togglePanels("withdrawCash")}>Withdraw Cash</a></li>
                        <li><a onClick={() => this.togglePanels("wiretransfer")}>Wire Transfer</a></li>
                    </ul>
                </div>
                <div id="initialPanel" className="actionPanel" style={{display: 'block'}}>
                    Select from left menu...
                </div>
                <div id="overdraftLimit" className="actionPanel" style={{display: 'none'}}>
                    <OverdraftLimit account={this.props.account}/>
                </div>
                <div id="wiretransferLimit" className="actionPanel" style={{display: 'none'}}>
                    <DailyWireTransferLimit account={this.props.account}/>
                </div>
                <div id="depositCheque" className="actionPanel" style={{display: 'none'}}>
                    <DepositCheque account={this.props.account}/>
                </div>
                <div id="depositCash" className="actionPanel" style={{display: 'none'}}>
                    <DepositCash account={this.props.account}/>
                </div>
                <div id="withdrawCash" className="actionPanel" style={{display: 'none'}}>
                    <WithdrawCash account={this.props.account} refreshAccountsData={this.props.refreshAccountsData}/>
                </div>
                <div id="wiretransfer" className="actionPanel" style={{display: 'none'}}>
                    <WireTransfer account={this.props.account} refreshAccountsData={this.props.refreshAccountsData}/>
                </div>
            </div>
        }
        return(
            <div>
                <a onClick={() => this.handleBackClick()}>Back</a>
                {component}
            </div>
        );
    }
}