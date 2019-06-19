import React, { Component } from 'react';

export class AccountDashboard extends Component {    
    render(){
        //console.log("account dashboard rendered");
        return(
            <div>
                <table className='table table-accounts-dashboard'>
                    <thead>
                        <tr>
                            <th colSpan="2">Account Dashboard</th>
                            <th colSpan="4">Account Id: [ {this.props.account[0].accountId} ]</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td style={{width: '65px'}}>Name:</td>
                            <td style={{width: '250px'}}>{this.props.account[0].accountHolderName}</td>
                            <td style={{width: '200px'}}>Overdraft limit</td>
                            <td style={{textAlign: 'right', width: '185px'}}>{new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(this.props.account[0].overdraftLimit)}</td>
                            <td>Account status:</td>
                            <td>{this.props.account[0].accountState === 0 ? <p>Active</p> : <p style={{color: 'red'}}>Blocked</p> }</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td>Daily wire transfer limit:</td>
                            <td style={{textAlign: 'right'}}>{new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(this.props.account[0].dailyWireTransferLimit)}</td>
                            <td>{this.props.account[0].accountState === 0 ? '' : 'Blocked due to:'}</td>
                            <td style={{color: 'red'}}>{this.props.account[0].reasonForAccountBlock}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td>Available balance:</td>
                            <td style={{textAlign: 'right'}}>
                                <p style={{color: this.props.account[0].availableFund < 0 ? 'red' : 'green'}}>
                                {new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(this.props.account[0].availableFund)}
                                </p>
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}